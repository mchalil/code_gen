#pragma once

typedef double tSamples;
typedef double tParamFlt;
typedef int tParamInt;
typedef int tParamFract;
typedef bool Bool;

typedef int Integer;
typedef int IndexInt;

typedef enum
{
	eLsAlgoStatus_ok,
	eLsAlgoStatus_err
}eLsAlgoStatus;
typedef struct _tLsBufferInfo
{
	Integer nBufferOffset;
	Integer nStride;
}tLsBufferInfo;

typedef struct _tLsPreProcInfo
{
	Integer nFrames;
}tLsPreProcInfo;

#include <math.h>
#define TICK_SZ (256)
#define MAX_STRIDE (1)
#define SCALE_BITS (15)
#define SCALE_FLT (1.0) // /(1<<SCALE_BITS))

#define LS_MULT(a, b) ((a)*(b)*SCALE_FLT)
#define LS_PI 3.14159265359
#define LS_2PI (2*3.14159265359)

extern tSamples aIOBufferArray[][TICK_SZ];


//#define GET_INPUT_PTR(file, sch, n)  &aIOBufferArray[pOO_##file##_##sch[n].nBufferOffset][0];
#define GET_OUTPUT_PTR(file, sch, n)  &aIOBufferArray[n][0];

#define xstr(s) str(s)
#define str(s) #s


#define PROCESS_CALL(file, sch) lss_##file##_##sch##((void*)&pInstance_##file##_##sch, pII_##file##_##sch, pOO_##file##_##sch)
#define INIT_CALL(file, sch) lss_##file##_##sch##_init((void*)&pInstance_##file##_##sch)
#define _SUGGEST_OUTPUTFILE_NAME(file, sch, n)  file_out_##file##_##sch##_##n##.txt
#define SUGGEST_OUTPUTFILE_NAME(file, sch, n) _SUGGEST_OUTPUTFILE_NAME(file, sch, n)
