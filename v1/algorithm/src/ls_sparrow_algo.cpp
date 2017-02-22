
#include "ls_code_gen_api.h"
#include "ls_sparrow_algo.h"


eLsAlgoStatus lss_module_mixmod(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;

	tSamples* pInput1 = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	tSamples* pInput2 = &aIOBufferArray[pInputOffsets[1].nBufferOffset][0];
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	int i = 0;
	for (i = 0; i < TICK_SZ; i++)
	{
		pOutput[i] = pInput1[i] * pInput2[i]; // LS_MULT(pInput1[i], pInput2[i]);
	}
	return status;
}

eLsAlgoStatus lss_module_root_sumsquares(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	tSamples* pInput_a = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	tSamples* pInput_b = &aIOBufferArray[pInputOffsets[1].nBufferOffset][0];
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	int i = 0;
	for (i = 0; i < TICK_SZ; i++)
	{
		pOutput[i] = sqrt(pInput_a[i] * pInput_a[i] + pInput_b[i] * pInput_b[i]);
	}

	return status;
}

eLsAlgoStatus lss_module_root_sumsquares2(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	tSamples* pInput_a = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	tSamples* pInput_b = &aIOBufferArray[pInputOffsets[1].nBufferOffset][0];
	tSamples* pOutput_p = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pOutput_n = &aIOBufferArray[pOutputOffset[1].nBufferOffset][0];
	int i = 0;
	for (i = 0; i < TICK_SZ; i++)
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

	return status;
}

eLsAlgoStatus lss_module_nco_bram_iq(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	nco_bram_iq_instance *pInstance = (nco_bram_iq_instance*)hInstance;
	tSamples* pOutput_s = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pOutput_c = &aIOBufferArray[pOutputOffset[1].nBufferOffset][0];
	int i = 0;
	for (i = 0; i < TICK_SZ; i++)
	{
		pOutput_s[i] = pInstance->amplitude*sin(pInstance->phstate);
		pOutput_c[i] = pInstance->amplitude*cos(pInstance->phstate);
		pInstance->phstate += LS_2PI*pInstance->frequency/pInstance->sampfreq;
	}

	return status;
}


eLsAlgoStatus lss_module_scaler(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	scaler_instance *pInstance = (scaler_instance*)hInstance;
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pInput = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];
	int i = 0;
	for (i = 0; i < TICK_SZ; i++)
	{
		pOutput[i] = pInstance->gain*pInput[i];
	}

	return status;
}

eLsAlgoStatus  lss_module_softfi(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_nco_bram(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)   
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	nco_bram_instance *pInstance = (nco_bram_instance*)hInstance;
	tSamples* pOutput_s = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	int i = 0;
	for (i = 0; i < TICK_SZ; i++)
	{
		pOutput_s[i] = pInstance->amplitude*sin(pInstance->phstate);
		pInstance->phstate += LS_2PI*pInstance->frequency / pInstance->sampfreq;
	}

	return status;
}

eLsAlgoStatus  lss_module_cicdec(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)	   
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_genfiraxi(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)  
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_decim(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset)	   
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	decim_instance *pInstance = (decim_instance*)hInstance;
	tSamples* pOutput = &aIOBufferArray[pOutputOffset[0].nBufferOffset][0];
	tSamples* pInput = &aIOBufferArray[pInputOffsets[0].nBufferOffset][0];

	int i = 0, j = 0;
	for (i = 0; i < TICK_SZ; i+= pInstance->nrate)
	{
		pOutput[j++] = pInput[i];
	}

	return status;
}

