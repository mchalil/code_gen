#include "ls_code_gen_api.h"

#define MAX_BUFFER_COUNT 13

typedef enum {
	 created,
	 inited,
	 err
} eState_ddc_hw_sw_ddc_software ;
typedef struct {
	 eState_ddc_hw_sw_ddc_software eState; 
}ddc_hw_sw_ddc_software_instance;
extern ddc_hw_sw_ddc_software_instance pInstance_ddc_hw_sw_ddc_software;
extern tLsBufferInfo pII_ddc_hw_sw_ddc_software[] ;
extern tLsBufferInfo pOO_ddc_hw_sw_ddc_software[] ;
#define GETOUT_PTR_0 &aIOBufferArray[9][0]; //PL_DDC_OUT
#define GETOUT_PTR_1 &aIOBufferArray[8][0]; //PS_DDC_OUT
extern char * outfileName0 ;
extern char * outfileName1 ;

extern tSamples aIOBufferArray[MAX_BUFFER_COUNT][TICK_SZ];


extern eLsAlgoStatus lss_ddc_hw_sw_ddc_software (void* hInstance, tLsBufferInfo* pInputOffsets, tLsBufferInfo* pOutputOffset) ;extern eLsAlgoStatus lss_ddc_hw_sw_ddc_software_init (void* hInstance) ;
