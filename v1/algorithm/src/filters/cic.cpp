// cic.cpp : Defines the entry point for the console application.
//
#if 0
#include "stdafx.h"
#define INTOUT(ii) S[ii-1]
#define COMBOUT(ii) compOut[ii-1]
#define MAX_DELAY_BUFF (2)
#define NUM_DELAY_BUFF (6)
#define DELAYBUF(jj, ii) delayBuf[MAX_DELAY_BUFF*(jj-1) + (ii-1)]
#define MAX_STAGES (32)

#include "gen.h"
#include "ls_math.h"

int CIC_n4_r4_m2(int xx, int *y);

int cic_ref(int *pX, int *pY, int r, int n, int BlockSize)
{
	int out = 0;
	for (int i = 0; i < BlockSize; i++)
	{
		int ret = CIC_n4_r4_m2(0x10, &pY[out]);
		if (ret == 1) 
			out++;
	}
	return 0;
}
int cic_int(int *pX, int *pY, int r, int n, int BlockSize)
{
	static int S[MAX_STAGES] = {0};
	int out = 0;
	static int count = 0;
	int countOut = 0;
	for (int i = 0; i < BlockSize; i++)
	{
		count++;
#if 0
		INTOUT(1) = INTOUT(1) + pX[i] / 4;
		INTOUT(2) = INTOUT(2) + INTOUT(1) / 2;
		INTOUT(3) = INTOUT(3) + INTOUT(2) / 2;
		INTOUT(4) = INTOUT(4) + INTOUT(3) / 2;
		INTOUT(5) = INTOUT(5) + INTOUT(4) / 2;
		INTOUT(6) = INTOUT(6) + INTOUT(5);

		pY[i] = INTOUT(6);
#else
		S[1] += pX[i];
		for (int j = 1; j < n; j++) {
			S[j+1] += S[j];
		}
#if 0
		S[1] += pX[i];
		S[2] += S[1];
		S[3] += S[2];
		S[4] += S[3];
		S[5] += S[4];
#endif
		if (count%r == 0)
		{
			static int compOut[MAX_STAGES];
			static int delayBuff[MAX_STAGES];

			delayBuff[1] = S[n-1]; // inp to the first comb
			for (int j = 1; j <= n; j++) {
				// using  matlab indexing. j = 0 is unused
				delayBuff[j+1] = delayBuff[j] - compOut[j]; 
				compOut[j] = delayBuff[j];
			}

#if 0
			delayBuff[2] = delayBuff[1] - compOut[1]; compOut[1] = delayBuff[1];
			delayBuff[3] = delayBuff[2] - compOut[2]; compOut[2] = delayBuff[2];
			delayBuff[4] = delayBuff[3] - compOut[3]; compOut[3] = delayBuff[3];
			delayBuff[5] = delayBuff[4] - compOut[4]; compOut[4] = delayBuff[4];
			delayBuff[6] = delayBuff[5] - compOut[5]; compOut[5] = delayBuff[5];
#endif
#if 0
			int z2 = z1 - compOut[1];
			int z3 = z2 - compOut[2];
			int z4 = z3 - compOut[3];
			int z5 = z4 - compOut[4];
			int z6 = z5 - compOut[5];
			compOut[1] = z1;
			compOut[2] = z2;
			compOut[3] = z3;
			compOut[4] = z4;
			compOut[5] = z5;
#endif
			pY[countOut++] = delayBuff[n];
		}


#endif


	}
	return 0;
}
void cic_down(int *pX, int *pY, int BlockSize, int r)
{
	int j = 0;
	for (int i = 0; i < BlockSize; i+=r)
	{
		pY[j++] = pX[i];
	}
}

int cic_comp(int *pX, int *pY, int BlockSize)
{
	static int compOut[MAX_STAGES];
	//static int delayBuf[NUM_DELAY_BUFF*MAX_DELAY_BUFF];
	for (int i = 0; i < BlockSize; i ++)
	{
#if 0
		// % combs
		COMBOUT(1) = pX[i] / 2 - DELAYBUF(1,2);
		DELAYBUF(1,2) = DELAYBUF(1,1);
		DELAYBUF(1,1) = pX[i] / 2;

		COMBOUT(2) = COMBOUT(1) - DELAYBUF(2,2);
		DELAYBUF(2, 2) = DELAYBUF(2,1);
		DELAYBUF(2, 1) = COMBOUT(1);

		COMBOUT(3) = COMBOUT(2) / 2 - DELAYBUF(3, 2);
		DELAYBUF(3, 2) = DELAYBUF(3, 1);
		DELAYBUF(3, 1) = COMBOUT(2) / 2;

		COMBOUT(4) = COMBOUT(3) / 2 - DELAYBUF(4, 2);
		DELAYBUF(4, 2) = DELAYBUF(4, 1);
		DELAYBUF(4, 1) = COMBOUT(3) / 2;

		COMBOUT(5) = COMBOUT(4) / 2 - DELAYBUF(5, 2);
		DELAYBUF(5, 2) = DELAYBUF(5,1);
		DELAYBUF(5,1) = COMBOUT(4) / 2;

		COMBOUT(6) = COMBOUT(5) / 4 - DELAYBUF(6, 2);
		DELAYBUF(6,2) = DELAYBUF(6,1);
		DELAYBUF(6,1) = COMBOUT(5) / 4;

		pY[i] = COMBOUT(6);
#else
		int z1 = pX[i]; // inp to the first comb


	 	int z2 = z1 - compOut[1];
		int z3 = z2 - compOut[2];
		int z4 = z3 - compOut[3];
		int z5 = z4 - compOut[4];
		int z6 = z5 - compOut[5];

		compOut[1] = z1;
		compOut[2] = z2;
		compOut[3] = z3;
		compOut[4] = z4;
		compOut[5] = z5;
		pY[i] = z5;
#endif
	}
	return 0;
}
int gen_impulse(int *pY, int BlockSize, int location)
{
	int static loc = 0;
	for (int i = 0; i < BlockSize; i++)
	{
		if (loc == location)
			pY[i] = ONE;
		else
			pY[i] = 0;
		loc++;
	}
	return 0;
}
int gen_ramp(int *pY, int BlockSize, int max)
{
	int static val = 0;
	for (int i = 0; i < BlockSize; i++)
	{
		if (val == max)
			val = 0;

			pY[i] = val;
			val += 1;// ONE;

	}
	return 0;
}
FILE *file_open(char *file)
{
	FILE *fp = fopen(file, "w");

	return fp;
}
void file_write(FILE *fp, int *pX, int BlockSize)
{
	for (int i = 0; i < BlockSize; i++)
	{
	//	fprintf(fp, "%d\n", pX[i] / ONE);
		fprintf(fp, "%d\n", pX[i]);
	}
}
int file_read(FILE *fp, int *pY, int BlockSize)
{
	int eof = 0;

	for (int i = 0; i < BlockSize; i++)
	{
		int ret = fscanf(fp, "%d\n", &pY[i]);
		if (ret != 1)
		{
			eof = 1;
			break;
		}
	//	pY[i] >>= (24 - BITS); // the file is with 24 bits
	}
	return eof;
}
void file_close(FILE *fp)
{
	if (fp == NULL) return;
	fclose(fp);
}
#include <stdio.h>
#include <stdlib.h>
#define BLOCK (32)
#define DOWN  (64)
int frameIn[BLOCK*DOWN];
int frameOut[BLOCK];

#include "SParrow.h"
#include "cic.h"

int ddc_process(int argc, char *argv[])
{
	FILE *fpIn;
	FILE *fpOut;
	FILE *fpGen;
	int r = 2;
	int n = 5;
	int f;

//	SParrow sp = new SParrow();

	if (argc != 6)
	{
		printf("ERROR : check the arguments\n");
		return -1;
	}
	else
	{
		char outfile[120];
		char infile[120];
		sprintf(infile, "%s.txt", argv[1]);
		sprintf(outfile, "%s_out.txt", argv[1]);
		fpIn = fopen(infile, "r");
		fpOut = fopen(outfile, "w");
		fpGen = fopen("genfile.txt", "w");
		r = atoi(argv[2]);
		if (r < 1 || r> 10000) {
			printf("ERROR : subsampling factor should be within (1 to 10000)");
			return -1;
		}
		n = atoi(argv[3]);
		if (n < 1 || n> 32) {
			printf("ERROR : stages(=n) factor should be within (1 to 32)");
			return -1;
		}
		f = atoi(argv[4]);
		if (f < 1 || f> 32000) {
			printf("ERROR : nco freq (=f) factor should be within (1 to 32000)");
			return -1;
		}
	}
	int InBlockSz = BLOCK*r;
	__int64 ww = 10000;
	for (__int64 j = 0; j < ww+100; j++)
	{
		gen_sin_fix(frameIn, InBlockSz, f, 48000);
	//	if(j > ww)
			file_write(fpGen, frameIn, InBlockSz);
	}
	for (int j = 0; j < 0; j++)
	{
		int eof = 0;
#if true
		// gen_ramp(frameIn, InBlockSz, 10*ONE);
		// gen_impulse(frameIn, InBlockSz, 0);
		gen_sin_fix(frameIn, InBlockSz, 121, 48000);
		file_write(fpGen, frameIn, InBlockSz);
#else
		eof = file_read(fpIn, frameIn, InBlockSz);
		if (eof == 1) {
			printf(" %df frames, total Samples Out/ In = %d / %d" , j, j*BLOCK, j*InBlockSz);
			break;
		}
#endif

	//	cic_ref(frameIn, frameOut, r, n, InBlockSz);
		cic_int(frameIn, frameOut, r, n, InBlockSz);
		file_write(fpOut, frameOut, BLOCK);
	}
	file_close(fpIn);
	file_close(fpOut);
	file_close(fpGen);
	return 0;
}



#endif
#include "cic.h"

cic::cic(int BlockSz, int NN, int RR, int MM)
{
	nCount = 0;
	BlockSize = BlockSz;
	N = NN;
	M = MM;
	R = RR;
	for (int i = 0; i < MAX_STAGES; i++)
	{
		State[i] = 0;
	}
	if (RR < 16)
	{   // pY[countOut++] = delayBuff[N] >> 4; // r <= 16
		pruning[0] = 1;
		pruning[1] = 4;
	}
	else if (RR < 20)
	{ // pY[countOut++] = delayBuff[N] >> 6;  // r = 20
		pruning[0] = 1;
		pruning[1] = 6;
	}
	else if (RR < 30) 
	{ // pY[countOut++] = delayBuff[N] >> 10;  // r = 30
		pruning[0] = 1;
		pruning[1] = 10;

	}
	else if (RR < 40)
	{ // pY[countOut++] = delayBuff[N] >> 14;  // r = 40
		pruning[0] = 1;
		pruning[1] = 14;
	}
	else
	{ //pY[countOut++] = delayBuff[N] >> 18;  // r = 64
		pruning[0] = 1;
		pruning[1] = 18;
	}
	
}


cic::~cic()
{
}
int cic::process(int *pX, int *pY)
{
	int countOut = 0;
	for (int i = 0; i < BlockSize; i++)
	{
		State[1] += pX[i]>> pruning[0];
#if 1
		for (int j = 1; j < N; j++) {
			State[j + 1] += State[j];
		}
#else
		// split for flexible pruning... 
		State[2] += State[1]/2;
		State[3] += State[2];
		State[4] += State[3];
		State[5] += State[4];
		State[6] += State[5];
#endif

#if 0


		INTOUT(1) = INTOUT(1) + pX[i] / 4;
		INTOUT(2) = INTOUT(2) + INTOUT(1) / 2;
		INTOUT(3) = INTOUT(3) + INTOUT(2) / 2;
		INTOUT(4) = INTOUT(4) + INTOUT(3) / 2;
		INTOUT(5) = INTOUT(5) + INTOUT(4) / 2;
		INTOUT(6) = INTOUT(6) + INTOUT(5);

		pY[i] = INTOUT(6);
#endif
		nCount++;

		if (nCount%R == 0)
		{
			delayBuff[1] = State[N - 1]; // inp to the first comb

			for (int j = 1; j <= N; j++) {
				// using  matlab indexing. j = 0 is unused
				delayBuff[j + 1] = delayBuff[j] - compOut[j];
				compOut[j] = delayBuff[j];
			}
			// pY[countOut++] = delayBuff[N] >> 4; // r <= 16
			// pY[countOut++] = delayBuff[N] >> 6;  // r = 20
			// pY[countOut++] = delayBuff[N] >> 10;  // r = 30
			// pY[countOut++] = delayBuff[N] >> 14;  // r = 40
			// pY[countOut++] = delayBuff[N] >> 18;  // r = 64
			pY[countOut++] = delayBuff[N] >> pruning[1];
		}
	}
	return 0;
}

