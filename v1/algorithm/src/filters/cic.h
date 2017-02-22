#pragma once
#define MAX_STAGES (8)
#define MAX_PRUNING_STAGES (4)

class cic
{
	tFract64 State[MAX_STAGES];
	tFract64 compOut[MAX_STAGES];
	tFract64 delayBuff[MAX_STAGES];

	Integer nCount;
	Integer BlockSize;
	Integer N, R, M;
	Integer pruning[MAX_PRUNING_STAGES];
public:
	cic(Integer BlockSz, Integer NN, Integer RR, Integer MM);
	~cic();
	Integer process(tFract32 *pX, tFract32 *pY);
	static Integer tst();
};

