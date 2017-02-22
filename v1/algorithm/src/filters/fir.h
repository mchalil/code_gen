#pragma once
#define MAX_FIR_ORDER (512) // todo

class fir
{
	int *pState;
	int *pCoef;
public:
	int nOrder;
	fir(int Order, int* pCoef);
	int process(int *pX, int* pY, int Samples );
	int State_write(int Sample);
	int fir::coef_read(char *fileName, int nSize,  int *pC);
	~fir();
};

