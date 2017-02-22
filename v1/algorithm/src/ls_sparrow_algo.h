#pragma once
eLsAlgoStatus ls_fw_automation_in(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);

eLsAlgoStatus  lss_module_nco_bram_iq(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_mixmod(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_root_sumsquares(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_root_sumsquares2(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_ls_fw_automation(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_scaler(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_softfi(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_nco_bram(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_cicdec(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_genfiraxi(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);
eLsAlgoStatus  lss_module_decim(void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset);

eLsAlgoStatus  lss_module_nco_bram_iq_init(void* hInstance);
eLsAlgoStatus  lss_module_mixmod_init(void* hInstance);
eLsAlgoStatus  lss_module_root_sumsquares_init(void* hInstance);
eLsAlgoStatus  lss_module_root_sumsquares2_init(void* hInstance);
eLsAlgoStatus  lss_module_ls_fw_automation_init(void* hInstance);
eLsAlgoStatus  lss_module_scaler_init(void* hInstance);
eLsAlgoStatus  lss_module_softfi_init(void* hInstance);
eLsAlgoStatus  lss_module_nco_bram_init(void* hInstance);
eLsAlgoStatus  lss_module_cicdec_init(void* hInstance);
eLsAlgoStatus  lss_module_genfiraxi_init(void* hInstance);
eLsAlgoStatus  lss_module_decim_init(void* hInstance);

eLsAlgoStatus lsAlgoCheck();

typedef enum {
	cicdec = 102,
	singleslice = 103
} lss_param;

typedef struct {

}softfi_instance;
typedef struct _nco_bram_iq_instance {
	Integer sample_width;
	Integer nbanks;
	tParamFlt amplitude;
	tParamFlt sampfreq;
	tParamFlt frequency;
	tParamFlt phstate;
}nco_bram_iq_instance;
typedef struct _nco_bram_instance {
	Integer sample_width;
	Integer nbanks;
	tParamFlt amplitude;
	tParamFlt sampfreq;
	tParamFlt frequency;
	tParamFlt phstate;
}nco_bram_instance;
typedef struct _mixmod_instance
{
	Bool modulate;
	Integer sample1_width;
	Integer sample2_width;
	Integer sample_out_width;
}mixmod_instance;
typedef struct _root_sumsquares_instance
{
	Integer sample_width;
}root_sumsquares_instance;

#include "cic.h"

typedef struct 
{
	lss_param filter_type;
	Integer sample_width;
	Integer nstage;
	Integer nrate;
	cic *pCic;
}cicdec_instance;

typedef struct {
	lss_param filter_type;
	Integer sample_width;
	Integer coeff_width;
	Integer ntap;
}genfiraxi_instance;

typedef struct {
	Integer nrate;
	Integer sample_width;
}decim_instance;
typedef struct _root_sumsquares2_instance
{
	Integer sample_width;
}root_sumsquares2_instance;

typedef struct _scaler_instance
{
	tParamFlt gain;
}scaler_instance;

typedef struct _ls_fw_automation_in_instance
{
	nco_bram_iq_instance oNco;
	nco_bram_iq_instance oNcoLf;

}ls_fw_automation_in_instance;
