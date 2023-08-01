#include <master_header.h>

#if CHECK_VERSION_APPL(1, 1, 2)

#define STR_FNAME_SIZE_WORK    (STR_FILE_NAME_SIZE - 16)

void transform_file_name(char *fname, char *initfn)
{
	unsigned16 slhp, cnt, c1;
	wchar_t *cmd_line;

	fname[STR_FNAME_SIZE_WORK-1] = '\0';
	slhp = 0;
	if (initfn[0] == '\\') {
		cnt = 1;
	} else {
		cmd_line = SysAllocString(GetCommandLine());
		c1 = 0;
		for (cnt = 0; cnt < STR_FNAME_SIZE_WORK; cnt++) {
			if (cmd_line[cnt] == '\0') break;
			if (cmd_line[cnt] != '"') {
				fname[c1] = (char)cmd_line[cnt];
				if (fname[c1] == '\\') slhp = c1+1;
				c1++;
			}
		}
		cnt = 0; 
	}
	while (slhp < STR_FNAME_SIZE_WORK) {
		if (initfn[cnt] == '\0') break;
		fname[slhp] = initfn[cnt];
		slhp++; cnt++;
	}
	while (slhp < STR_FNAME_SIZE_WORK) {
		fname[slhp] = '\0';
		slhp++;
	}
	if (fname[STR_FNAME_SIZE_WORK-1] != '\0') {
    printf("Program path too long\n");
    fname[STR_FNAME_SIZE_WORK-1] = '\0';
  }
}

void time_stamp_file_name(char *fname, char *initfn, time_t ts)		// 1.1.2 API changed
{
	unsigned16 cnt, pntp, fnap, tsym;
	struct tm tp;
	char tstxt[STR_TS_SIZE];

	tp = *localtime(&ts);
	strftime(tstxt, STR_TS_SIZE, "_%Y%m%d_%H%M%S", &tp);		// 16 symbols
	tsym = 16;
	pntp = 0;
	fnap = 0;
	for (cnt = 0; cnt < STR_FILE_NAME_SIZE-1; cnt++) {
		if (initfn[cnt] == '\0') break;
		fname[fnap] = initfn[cnt];
		if (initfn[cnt] == '.') pntp = cnt+1;
		fnap++;
	}
	if (pntp > 0) fnap = pntp - 1;
	if (fnap >= STR_FILE_NAME_SIZE-1-tsym) fnap = STR_FILE_NAME_SIZE-1-tsym;
	for (cnt = 0; cnt < tsym; cnt++) {
		fname[fnap] = tstxt[cnt];
		fnap++;
	}
	if (pntp > 0) {
		pntp -= 1;
		while (fnap < STR_FILE_NAME_SIZE-1) {
			if (initfn[pntp] == '\0') break;
			fname[fnap] = initfn[pntp];
			fnap++; pntp++;
		}
	}
	while (fnap < STR_FILE_NAME_SIZE-1) {
		fname[fnap] = '\0';
		fnap++;
	}
	fname[STR_FILE_NAME_SIZE-1] = '\0';
}

#endif
