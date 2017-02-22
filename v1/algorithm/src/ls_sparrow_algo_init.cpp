#include "ls_code_gen_api.h"
#include "ls_sparrow_algo.h"

eLsAlgoStatus  lss_module_nco_bram_iq_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_mixmod_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_root_sumsquares_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_root_sumsquares2_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_ls_fw_automation_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_scaler_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_softfi_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_nco_bram_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
eLsAlgoStatus  lss_module_cicdec_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	cicdec_instance *pCicModule = (cicdec_instance*)hInstance;
	pCicModule->pCic = new cic(TICK_SZ, pCicModule->nrate, pCicModule->nstage, 1);
	return status;
}
eLsAlgoStatus  lss_module_genfiraxi_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	genfiraxi_instance *pFirModule = (genfiraxi_instance*)hInstance;

	pFirModule->pFir = new fir(5, pFirModule->pFIRCoeff);


	return status;
}
eLsAlgoStatus  lss_module_decim_init(void* hInstance)
{
	eLsAlgoStatus status = eLsAlgoStatus_ok;
	return status;
}
