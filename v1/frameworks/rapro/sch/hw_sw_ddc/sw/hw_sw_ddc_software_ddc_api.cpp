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


softfi_instance softfi_psfi_1 = {};
 nco_bram_instance nco_bram_ps_nco = { 16 /* sample_width */, };
mixmod_instance mixmod_ps_mixer = { 0 /* modulate */, 16 /* sample1_width */, 16 /* sample2_width */, 16 /* sample_out_width */, };
cicdec_instance cicdec_ps_cicdec = { cicdec /* filter_type */, 16 /* sample_width */, 5 /* nstage */, 24 /* nrate */, };
genfiraxi_instance genfiraxi_ps_cfir = { singleslice /* filter_type */, 16 /* sample_width */, 18 /* coeff_width */, 24 /* ntap */, };
decim_instance decim_ps_cfir_dec = { 2 /* nrate */, 16 /* sample_width */, };
genfiraxi_instance genfiraxi_ps_pfir = { singleslice /* filter_type */, 16 /* sample_width */, 18 /* coeff_width */, 48 /* ntap */, };
decim_instance decim_ps_pfir_dec = { 2 /* nrate */, 16 /* sample_width */, };
softfi_instance softfi_psfi_2 = {};
 
ddc_hw_sw_ddc_software_instance pInstance_ddc_hw_sw_ddc_software = { created };
tLsBufferInfo pII_ddc_hw_sw_ddc_software[] = {{0, 1}}; // null; // 0
tLsBufferInfo pOO_ddc_hw_sw_ddc_software[] = { {9, 1},{8, 1} }; // 10
char * outfileName0 = xstr(SUGGEST_OUTPUTFILE_NAME(ddc, hw_sw_ddc_software, 9));
char * outfileName1 = xstr(SUGGEST_OUTPUTFILE_NAME(ddc, hw_sw_ddc_software, 8));

tSamples aIOBufferArray[MAX_BUFFER_COUNT][TICK_SZ];


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

