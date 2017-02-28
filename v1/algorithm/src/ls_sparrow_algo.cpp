
#include "ls_code_gen_api.h"
#include "ls_sparrow_algo.h"


eLsAlgoStatus lss_module_mixmod(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	tSamples* pInput1 = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	tSamples* pInput2 = &aIOBufferArray[pInputOffsets[1].nBufferOffset][0];
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	Integer inStride = aIOBufferStride[pInputOffsets[0].nBufferOffset];
	IndexInt i = 0;
	for (i = 0; i < TICK_SZ; i+= inStride)
	{
		pOutput[i] = (pInput1[i] * pInput2[i]); // LS_MULT(pInput1[i], pInput2[i]);
	}
	aIOBufferStride[pOutputOffset[0].nBufferOffset] = inStride;
#if 1
	static int c = 0;
	c += TICK_SZ;
	if (c >= 76800)
	{
		c = c;
	}
#endif
	return status;
}

eLsAlgoStatus lss_module_root_sumsquares(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	tSamples* pInput_a = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	tSamples* pInput_b = &aIOBufferArray[pInputOffsets[1].nBufferOffset][0];
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	Integer inStride = aIOBufferStride[pInputOffsets[0].nBufferOffset];
	IndexInt i = 0;
	for (i = 0; i < TICK_SZ; i+=inStride)
	{
		pOutput[i] = sqrt(pInput_a[i] * pInput_a[i] + pInput_b[i] * pInput_b[i]);
	}
	aIOBufferStride[pOutputOffset[0].nBufferOffset] = inStride;
	return status;
}

eLsAlgoStatus lss_module_root_sumsquares2(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	tSamples* pInput_a = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	tSamples* pInput_b = &aIOBufferArray[pInputOffsets[1].nBufferOffset][0];
	tSamples* pOutput_p = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pOutput_n = &aIOBufferArray[pOutputOffset[1].nBufferOffset][0];
	Integer inStride = aIOBufferStride[pInputOffsets[0].nBufferOffset];

	IndexInt i = 0;
	for (i = 0; i < TICK_SZ; i+= inStride)
	{
		if (pInput_a[i] > 0) {
			pOutput_p[i] = sqrt(pInput_a[i] * pInput_a[i] + pInput_b[i] * pInput_b[i]);
			pOutput_n[i] = 0;
		}
		else {
			pOutput_p[i] = 0;
			pOutput_n[i] = -sqrt(pInput_a[i] * pInput_a[i] + pInput_b[i] * pInput_b[i]);
		}
	}
	aIOBufferStride[pOutputOffset[0].nBufferOffset] = inStride;
	aIOBufferStride[pOutputOffset[1].nBufferOffset] = inStride;  // same for both input channels. 

	return status;
}

eLsAlgoStatus lss_module_nco_bram_iq(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	nco_bram_iq_instance *pInstance = (nco_bram_iq_instance*)hInstance;
	tSamples* pOutput_s = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pOutput_c = &aIOBufferArray[pOutputOffset[1].nBufferOffset][0];
	Integer inStride = pInputOffsets->nStride;
	IndexInt i = 0;
	for (i = 0; i < TICK_SZ; i+= inStride)
	{
		pOutput_s[i] = pInstance->amplitude*sin(pInstance->phstate);
		pOutput_c[i] = pInstance->amplitude*cos(pInstance->phstate);
		pInstance->phstate += LS_2PI*pInstance->frequency/pInstance->sampfreq;
	}

	aIOBufferStride[pOutputOffset[0].nBufferOffset] = inStride;

	return status;
}

eLsAlgoStatus lss_module_scaler(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	scaler_instance *pInstance = (scaler_instance*)hInstance;
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pInput = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	IndexInt i = 0;
	Integer inStride = aIOBufferStride[pInputOffsets[0].nBufferOffset];

	for (i = 0; i < TICK_SZ; i+= inStride)
	{
		pOutput[i] = pInstance->gain*pInput[i];
	}
	aIOBufferStride[pOutputOffset[0].nBufferOffset] = inStride;

	return status;
}

eLsAlgoStatus  lss_module_softfi(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	//              out1.data = sin(2*pi()*[1:h.lssys.tick]*13.51e6/h.lssys.sampfreq);
	softfi_instance *pInstance = (softfi_instance*)hInstance;
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	Integer inStride = pInputOffsets->nStride;
	IndexInt i = 0;
	for (i = 0; i < TICK_SZ; i+= inStride)
	{
#ifndef USE_VALUE_FROM_FILE
		fscanf_s(pInstance->pFile, "%lf", &pOutput[i]);
#else
		static int count = 0;
		pOutput[i] = count++;// sin(2 * 3.14*13510000.0*c++ / 250000000.0);
#endif
	}
	aIOBufferStride[pOutputOffset[0].nBufferOffset] = inStride;

	return status;
}

eLsAlgoStatus  lss_module_nco_bram(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)   
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	nco_bram_instance *pInstance = (nco_bram_instance*)hInstance;
	tSamples* pOutput_s = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	Integer inStride = pInputOffsets->nStride;
	IndexInt i = 0;
	for (i = 0; i < TICK_SZ; i+=inStride)
	{
		pOutput_s[i] =pInstance->amplitude*sin(pInstance->phstate);
		pInstance->phstate += LS_2PI*pInstance->frequency / pInstance->sampfreq;
	}
	aIOBufferStride[pOutputOffset[0].nBufferOffset] = inStride;

	return status;
}

eLsAlgoStatus  lss_module_cicdec(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)	   
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;

	cicdec_instance *pCicModule = (cicdec_instance*)hInstance;
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pInput = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];

	pCicModule->pCic->process(pInput, pOutput);
	aIOBufferStride[pOutputOffset[0].nBufferOffset] = aIOBufferStride[pInputOffsets[0].nBufferOffset] * pCicModule->nrate;

	return status;
}

eLsAlgoStatus  lss_module_genfiraxi(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)  
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	genfiraxi_instance *pFirModule = (genfiraxi_instance*)hInstance;
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pInput = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	Integer inStride = aIOBufferStride[pInputOffsets[0].nBufferOffset];

#ifndef BYPASS_FIR
	pFirModule->pFir->process(pInput, pOutput, TICK_SZ, inStride);
#else
	for (int j = 0; j < TICK_SZ; j += inStride)
	{
		pOutput[j] = pInput[j];
	}
#endif
	aIOBufferStride[pOutputOffset[0].nBufferOffset] = inStride;

	return status;
}

eLsAlgoStatus  lss_module_decim(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)	   
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	decim_instance *pInstance = (decim_instance*)hInstance;
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pInput = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	Integer inStride = aIOBufferStride[pInputOffsets[0].nBufferOffset];
	Integer outStride = inStride*pInstance->nrate;
	IndexInt j = 0;
	for (j = 0; j < TICK_SZ; j+= outStride)
	{
		pOutput[j] = pInput[j];
	}
	aIOBufferStride[pOutputOffset[0].nBufferOffset] = outStride;

	return status;
}

