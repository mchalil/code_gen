#pragma once
#define MAX_STAGES (8)
#define MAX_PRUNING_STAGES (4)

class cic
{
	__int64 State[MAX_STAGES];
	__int64 compOut[MAX_STAGES];
	__int64 delayBuff[MAX_STAGES];

	int nCount;
	int BlockSize;
	int N, R, M;
	int pruning[MAX_PRUNING_STAGES];
public:
	cic(int BlockSz, int NN, int RR, int MM);
	~cic();
	int process(int *pX, int *pY);
};

