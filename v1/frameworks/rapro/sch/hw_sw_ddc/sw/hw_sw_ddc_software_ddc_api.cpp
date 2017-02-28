#include "ls_code_gen_api.h"
#include "ls_sparrow_algo.h"
#include "hw_sw_ddc_software_ddc_api.h"

tLsBufferInfo pII_softfi_psfi_1[] = { {0, 1} };
tLsBufferInfo pOO_softfi_psfi_1[] = { {1, 1} }; // 1

tLsBufferInfo pII_nco_bram_ps_nco[] = { {0, 1} };
tLsBufferInfo pOO_nco_bram_ps_nco[] = { {2, 1} }; // 2

tLsBufferInfo pII_mixmod_ps_mixer[] = { {2, 1},{1, 1} };
tLsBufferInfo pOO_mixmod_ps_mixer[] = { {3, 1} }; // 3

tLsBufferInfo pII_cicdec_ps_cicdec[] = { {3, 1} };
tLsBufferInfo pOO_cicdec_ps_cicdec[] = { {4, 1} }; // 4

tLsBufferInfo pII_genfiraxi_ps_cfir[] = { {4, 1} };
tLsBufferInfo pOO_genfiraxi_ps_cfir[] = { {5, 1} }; // 5

tLsBufferInfo pII_decim_ps_cfir_dec[] = { {5, 1} };
tLsBufferInfo pOO_decim_ps_cfir_dec[] = { {6, 1} }; // 6

tLsBufferInfo pII_genfiraxi_ps_pfir[] = { {6, 1} };
tLsBufferInfo pOO_genfiraxi_ps_pfir[] = { {7, 1} }; // 7

tLsBufferInfo pII_decim_ps_pfir_dec[] = { {7, 1} };
tLsBufferInfo pOO_decim_ps_pfir_dec[] = { {8, 1} }; // 8

tLsBufferInfo pII_softfi_psfi_2[] = { {0, 1} };
tLsBufferInfo pOO_softfi_psfi_2[] = { {9, 1} }; // 9


softfi_instance softfi_psfi_1 =  {pOO_softfi_psfi_1};
//Reading from param file ..\..\..\..\data\nco_bram_ps_nco.xml; 
nco_bram_instance nco_bram_ps_nco = { 
 18, /* sample_width */
 2, /* nbanks (?) */
 1000, /* amplitude */
 245760000, /* sampfreq */
 13500000, /* freq */
 0, /* phstate (?) */
}; 
mixmod_instance mixmod_ps_mixer = { 0 /* modulate */, 16 /* sample1_width */, 16 /* sample2_width */, 16 /* sample_out_width */, };
//Reading from param file ..\..\..\..\data\cicdec_ps_cicdec.xml; 
cicdec_instance cicdec_ps_cicdec = { cicdec, 18, 24, 5 };
//Reading from param file ..\..\..\..\data\genfiraxi_ps_cfir.xml; 
tParamFract pFirCoeff_genfiraxi_ps_cfir[] = {
-79,-469,-584,375,2241,2443,-1680,-7839,-7818,5316,27973,45930,45930,27973,5316,-7818,-7839,-1680,2443,2241,375,-584,-469,-79};
genfiraxi_instance genfiraxi_ps_cfir = { singleslice, 
 17 /* sample_width (?) */,
 18 /* coeff_width (?) */,
 24 /* ntap */,
 pFirCoeff_genfiraxi_ps_cfir };
decim_instance decim_ps_cfir_dec = { 2 /* nrate */, 16 /* sample_width */, };
//Reading from param file ..\..\..\..\data\genfiraxi_ps_pfir.xml; 
tParamFract pFirCoeff_genfiraxi_ps_pfir[] = {
-63,-6,70,171,290,402,461,410,197,-203,-767,-1414,-2001,-2342,-2235,-1505,-48,2139,4932,8088,11274,14113,16245,17389,17389,16245,14113,11274,8088,4932,2139,-48,-1505,-2235,-2342,-2001,-1414,-767,-203,197,410,461,402,290,171,70,-6,-63};
genfiraxi_instance genfiraxi_ps_pfir = { singleslice, 
 17 /* sample_width (?) */,
 18 /* coeff_width (?) */,
 48 /* ntap */,
 pFirCoeff_genfiraxi_ps_pfir };
decim_instance decim_ps_pfir_dec = { 2 /* nrate */, 16 /* sample_width */, };
softfi_instance softfi_psfi_2 =  {pOO_softfi_psfi_2};

ddc_hw_sw_ddc_software_instance pInstance_ddc_hw_sw_ddc_software = { created };
tLsBufferInfo pII_ddc_hw_sw_ddc_software[] = {{0, 1}}; // null; // 0
tLsBufferInfo pOO_ddc_hw_sw_ddc_software[] = { {9, 1},{8, 1} }; // 10
char * outfileName0 = xstr(SUGGEST_OUTPUTFILE_NAME(ddc, hw_sw_ddc_software, 9));
char * outfileName1 = xstr(SUGGEST_OUTPUTFILE_NAME(ddc, hw_sw_ddc_software, 8));

tSamples aIOBufferArray[MAX_BUFFER_COUNT][TICK_SZ];
Integer aIOBufferStride[MAX_BUFFER_COUNT];


eLsAlgoStatus lss_ddc_hw_sw_ddc_software (void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset) 
{ 
	eLsAlgoStatus status = eLsAlgoStatus_ok; 

	lss_module_softfi ((void*)&softfi_psfi_1, pII_softfi_psfi_1, pOO_softfi_psfi_1); // 1
	lss_module_nco_bram ((void*)&nco_bram_ps_nco, pII_nco_bram_ps_nco, pOO_nco_bram_ps_nco); // 2
	lss_module_mixmod ((void*)&mixmod_ps_mixer, pII_mixmod_ps_mixer, pOO_mixmod_ps_mixer); // 3
	lss_module_cicdec ((void*)&cicdec_ps_cicdec, pII_cicdec_ps_cicdec, pOO_cicdec_ps_cicdec); // 4
	lss_module_genfiraxi ((void*)&genfiraxi_ps_cfir, pII_genfiraxi_ps_cfir, pOO_genfiraxi_ps_cfir); // 5
	lss_module_decim ((void*)&decim_ps_cfir_dec, pII_decim_ps_cfir_dec, pOO_decim_ps_cfir_dec); // 6
	lss_module_genfiraxi ((void*)&genfiraxi_ps_pfir, pII_genfiraxi_ps_pfir, pOO_genfiraxi_ps_pfir); // 7
	lss_module_decim ((void*)&decim_ps_pfir_dec, pII_decim_ps_pfir_dec, pOO_decim_ps_pfir_dec); // 8
	lss_module_softfi ((void*)&softfi_psfi_2, pII_softfi_psfi_2, pOO_softfi_psfi_2); // 9
return status;
}
eLsAlgoStatus lss_ddc_hw_sw_ddc_software_init (void* hInstance) 
{ 
	eLsAlgoStatus status = eLsAlgoStatus_ok; 

	lss_module_softfi_init ((void*)&softfi_psfi_1); // 1
	lss_module_nco_bram_init ((void*)&nco_bram_ps_nco); // 2
	lss_module_mixmod_init ((void*)&mixmod_ps_mixer); // 3
	lss_module_cicdec_init ((void*)&cicdec_ps_cicdec); // 4
	lss_module_genfiraxi_init ((void*)&genfiraxi_ps_cfir); // 5
	lss_module_decim_init ((void*)&decim_ps_cfir_dec); // 6
	lss_module_genfiraxi_init ((void*)&genfiraxi_ps_pfir); // 7
	lss_module_decim_init ((void*)&decim_ps_pfir_dec); // 8
	lss_module_softfi_init ((void*)&softfi_psfi_2); // 9
return status;
}
eLsAlgoStatus lss_ddc_hw_sw_ddc_software_close (void* hInstance) 
{ 
	eLsAlgoStatus status = eLsAlgoStatus_ok; 

	lss_module_softfi_close ((void*)&softfi_psfi_1); // 1
	lss_module_nco_bram_close ((void*)&nco_bram_ps_nco); // 2
	lss_module_mixmod_close ((void*)&mixmod_ps_mixer); // 3
	lss_module_cicdec_close ((void*)&cicdec_ps_cicdec); // 4
	lss_module_genfiraxi_close ((void*)&genfiraxi_ps_cfir); // 5
	lss_module_decim_close ((void*)&decim_ps_cfir_dec); // 6
	lss_module_genfiraxi_close ((void*)&genfiraxi_ps_pfir); // 7
	lss_module_decim_close ((void*)&decim_ps_pfir_dec); // 8
	lss_module_softfi_close ((void*)&softfi_psfi_2); // 9
return status;
}

