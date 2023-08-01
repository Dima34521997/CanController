#include <master_header.h>

#if CHECK_VERSION_APPL(1, 1, 1)

static int16 sectype, sectflag;
static unsigned16 cfgline;
static unsigned32 mask_sections, mask_tokens;
static cannode cfgnode;

static void reset_section(int16 error)
{
	unsigned8 et;

	if (error < 0) {
		sectflag = -1;
		et = EVENT_TYPE_ERROR;
	} else {
		sectflag = 0;
		et = EVENT_TYPE_WARNING;
	}
	master_event(EVENT_CLASS_MASTER_CONFIG, et, error, cfgline);
	sectype = CFG_SECTION_NONE;
}

static void invalid_token(void)
{
	master_event(EVENT_CLASS_MASTER_CONFIG, EVENT_TYPE_WARNING, CFG_WARNING_INV_TOKEN, cfgline);
}

static void invalid_value(void)
{
	master_event(EVENT_CLASS_MASTER_CONFIG, EVENT_TYPE_WARNING, CFG_WARNING_INV_VALUE, cfgline);
}

static void undefined_sections(void)
{
	if (mask_sections & MASK_SECTION_FILENAMES) {
		master_event(EVENT_CLASS_MASTER_CONFIG, EVENT_TYPE_ERROR, CFG_ERROR_UNDEF_FILENAMES, 0);
	}
	if (mask_sections & MASK_SECTION_CANNETWORK) {
		master_event(EVENT_CLASS_MASTER_CONFIG, EVENT_TYPE_ERROR, CFG_ERROR_UNDEF_CANNETWORK, 0);
	}
}

static void duplicated_sections(unsigned32 mask)
{
	if (mask_sections & mask) {
		mask_sections &= ~mask;
		return;
	}
	if (mask == MASK_SECTION_FILENAMES) reset_section(CFG_WARNING_DUPL_FILENAMES);
	else if (mask == MASK_SECTION_CANNETWORK) reset_section(CFG_WARNING_DUPL_CANNETWORK);
}

static int16 duplicated_token(unsigned32 mask)
{
	if (mask_tokens & mask) {
		mask_tokens &= ~mask;
		return GEN_RETOK;
	}
	master_event(EVENT_CLASS_MASTER_CONFIG, EVENT_TYPE_WARNING, CFG_WARNING_DUPL_TOKEN, cfgline);
	return GEN_ERRET_DUPLICATED;
}

static void process_comments(char *line)
{
}

static void undefined_filenames_tokens(void)
{
	if (mask_tokens & MASK_TOKEN_LOGFILE) {
		master_event(EVENT_CLASS_MASTER_CONFIG, EVENT_TYPE_ERROR, CFG_ERROR_UNDEF_LOGFILE, 0);
	}
}

static void filenames_section(char *line)
{
	unsigned16 npos;
	char fname[STR_FILE_NAME_SIZE];

	npos = 0;
	if (find_token("LOGFILE", line, &npos) == GEN_RETOK) {
		if (duplicated_token(MASK_TOKEN_LOGFILE) != GEN_RETOK) return;
		if (parse_string(fname, line+npos, '=', STR_FILE_NAME_SIZE, &npos) == GEN_RETOK) {
			clip_string(fname, STR_FILE_NAME_SIZE);
			transform_file_name(log_file_name, fname);
			return;
		}
		invalid_value();
		return;
	}
	invalid_token();
}

static void undefined_network_tokens(void)
{
	if (mask_tokens & MASK_TOKEN_NETWORK) {
		master_event(EVENT_CLASS_MASTER_CONFIG, EVENT_TYPE_ERROR, CFG_ERROR_UNDEF_NETWORK, 0);
	}
	if (mask_tokens & MASK_TOKEN_BITRATE) {
		master_event(EVENT_CLASS_MASTER_CONFIG, EVENT_TYPE_ERROR, CFG_ERROR_UNDEF_BITRATE, 0);
	}
}

static void network_section(char *line)
{
	unsigned16 npos;
	union numbers numtmp;

	npos = 0;
	if (find_token("NETWORK", line, &npos) == GEN_RETOK) {
		if (duplicated_token(MASK_TOKEN_NETWORK) != GEN_RETOK) return;
		if (parse_integer(&numtmp, line+npos, CAN_DEFTYPE_UNSIGNED8, 0, &npos) == GEN_RETOK) {
			if (numtmp.uns8 >= CAN_NETWORK_MIN && numtmp.uns8 <= CAN_NETWORK_MAX) {
				can_network = numtmp.uns8;
				return;
			}
		}
		invalid_value();
		return;
	}
	npos = 0;
	if (find_token("BITRATEINDEX", line, &npos) == GEN_RETOK) {
		if (duplicated_token(MASK_TOKEN_BITRATE) != GEN_RETOK) return;
		if (parse_integer(&numtmp, line+npos, CAN_DEFTYPE_UNSIGNED8, 0, &npos) == GEN_RETOK) {
			if (check_bitrate_index(numtmp.uns8) == CAN_RETOK) {
				bitrate_index = numtmp.uns8;
				return;
			}
		}
		invalid_value();
		return;
	}
	invalid_token();
}

static void cannode_section(char *line)
{
	unsigned16 npos;
	union numbers numtmp;

	npos = 0;
	if (find_token("NODEID", line, &npos) == GEN_RETOK) {
		if (duplicated_token(MASK_TOKEN_NODEID) != GEN_RETOK) return;
		if (parse_integer(&numtmp, line+npos, CAN_DEFTYPE_UNSIGNED8, 0, &npos) == GEN_RETOK) {
			if (check_node_id(numtmp.uns8) == CAN_RETOK) {
				cfgnode = numtmp.uns8;
				if (can_node[cfgnode].node_status == ON) reset_section(CFG_WARNING_DUPL_NODE);
				else can_node[cfgnode].node_status = ON;
				return;
			}
		}
		reset_section(CFG_WARNING_INV_NODE);
		return;
	}
	if (cfgnode == 0) {
		reset_section(CFG_WARNING_UNDEF_NODE);
		return;
	}
	npos = 0;
	if (find_token("DEVICETYPE", line, &npos) == GEN_RETOK) {
		if (duplicated_token(MASK_TOKEN_DEVICETYPE) != GEN_RETOK) return;
		if (parse_integer(&numtmp, line+npos, CAN_DEFTYPE_UNSIGNED32, 0, &npos) == GEN_RETOK) {
			can_node[cfgnode].DeviceType = numtmp.uns32;
			can_node[cfgnode].maskdev |= MASK_DEV_DEVICETYPE;
			return;
		}
		invalid_value();
		return;
	}
	npos = 0;
	if (find_token("VENDORID", line, &npos) == GEN_RETOK) {
		if (duplicated_token(MASK_TOKEN_VENDORID) != GEN_RETOK) return;
		if (parse_integer(&numtmp, line+npos, CAN_DEFTYPE_UNSIGNED32, 0, &npos) == GEN_RETOK) {
			can_node[cfgnode].VendorID = numtmp.uns32;
			can_node[cfgnode].maskdev |= MASK_DEV_VENDORID;
			return;
		}
		invalid_value();
		return;
	}
	npos = 0;
	if (find_token("PRODUCTCODE", line, &npos) == GEN_RETOK) {
		if (duplicated_token(MASK_TOKEN_PRODCODE) != GEN_RETOK) return;
		if (parse_integer(&numtmp, line+npos, CAN_DEFTYPE_UNSIGNED32, 0, &npos) == GEN_RETOK) {
			can_node[cfgnode].ProductCode = numtmp.uns32;
			can_node[cfgnode].maskdev |= MASK_DEV_PRODUCTCODE;
			return;
		}
		invalid_value();
		return;
	}
	npos = 0;
	if (find_token("REVISION", line, &npos) == GEN_RETOK) {
		if (duplicated_token(MASK_TOKEN_REVISION) != GEN_RETOK) return;
		if (parse_integer(&numtmp, line+npos, CAN_DEFTYPE_UNSIGNED32, 0, &npos) == GEN_RETOK) {
			can_node[cfgnode].Revision = numtmp.uns32;
			can_node[cfgnode].maskdev |= MASK_DEV_REVISION;
			return;
		}
		invalid_value();
		return;
	}
	npos = 0;
	if (find_token("SERIAL", line, &npos) == GEN_RETOK) {
		if (duplicated_token(MASK_TOKEN_SERIAL) != GEN_RETOK) return;
		if (parse_integer(&numtmp, line+npos, CAN_DEFTYPE_UNSIGNED32, 0, &npos) == GEN_RETOK) {
			can_node[cfgnode].Serial = numtmp.uns32;
			can_node[cfgnode].maskdev |= MASK_DEV_SERIAL;
			return;
		}
		invalid_value();
		return;
	}
	invalid_token();
}

static void parse_section(char *line)
{
	if (sectype == CFG_SECTION_NODEID) cannode_section(line);
	else if (sectype == CFG_SECTION_NETWORK) network_section(line);
	else if (sectype == CFG_SECTION_FILENAMES) filenames_section(line);
	else if (sectype == CFG_SECTION_COMMENTS) process_comments(line);
}

static void open_section(char *line)
{
	unsigned16 npos;

	if (sectflag < 0) return;
	sectflag = 1;
	npos = 0;
	if (find_token("[CANOPENNODE]", line, &npos) == GEN_RETOK) {
		cfgnode = 0;
		mask_tokens = MASK_TOKENS_CANNODE;
		sectype = CFG_SECTION_NODEID;
		return;
	}
	if (find_token("[CANNETWORK]", line, &npos) == GEN_RETOK) {
		sectype = CFG_SECTION_NETWORK;
		mask_tokens = MASK_TOKENS_CANNETWORK;
		duplicated_sections(MASK_SECTION_CANNETWORK);
		return;
	}
	if (find_token("[FILENAMES]", line, &npos) == GEN_RETOK) {
		sectype = CFG_SECTION_FILENAMES;
		mask_tokens = MASK_TOKENS_FILENAMES;
		duplicated_sections(MASK_SECTION_FILENAMES);
		return;
	}
	if (find_token("[COMMENTS]", line, &npos) == GEN_RETOK) {
		sectype = CFG_SECTION_COMMENTS;
		return;
	}
	reset_section(CFG_WARNING_INV_SECTION);
}

static void close_section(void)
{
	if (sectflag <= 0) return;
	sectflag = 0;
	if (sectype == CFG_SECTION_NETWORK) undefined_network_tokens();
	else if (sectype == CFG_SECTION_FILENAMES) undefined_filenames_tokens();
	sectype = CFG_SECTION_NONE;
}

static int16 file_version(char *line)
{
	unsigned16 npos;
	union numbers num;

	npos = 0;
	if (find_token("[PCFG", line, &npos) == GEN_RETOK) {
		if (parse_integer(&num, line+npos, CAN_DEFTYPE_UNSIGNED32, 16, &npos) == GEN_RETOK) {
			if (num.uns32 == MASTER_CONFIG_FILE_VERSION) return GEN_RETOK;
		}
	}
	reset_section(CFG_ERROR_VERSION);
	return CFG_ERROR_VERSION;
}

void read_config(void)
{
	FILE *filpds;
	char file_name[STR_FILE_NAME_SIZE];
	char filine[STR_LINE_SIZE];
	char scur, sprev;
	int32 buf;
	unsigned16 fver, fcom, clin, npos;

	transform_file_name(file_name, MASTER_CONFIG_FILE_NAME);
	filpds = fopen(file_name, "r");
	if (filpds == NULL) {
		master_event(EVENT_CLASS_MASTER_CONFIG, EVENT_TYPE_ERROR, EVENT_CODE_FILE_OPEN, EVENT_INFO_DUMMY);
		return;
	}
	cfgline = 0;
	sectflag = 0;
	sectype = CFG_SECTION_NONE;
	mask_sections = MASK_CONFIG_SECTIONS;
	mask_tokens = 0;
	npos = 0;
	fver = 0;
	clin = 0;
	fcom = 0;
	sprev = '\0';
	scur = '\0';
	filine[0] = '\0';
	while (TRUE) {
		sprev = scur;
		buf = fgetc(filpds);
		if (buf == EOF) scur = '\0';
		else scur = (char)buf;
		if (scur == '\0' || scur == '\n') {
			cfgline++;
			if (clin > 0) {
				filine[clin] = '\0';
				if (fver == 0) {
					if (file_version(filine) != GEN_RETOK) break;
					fver = 1;
				} else {
					if (filine[0] == '[') {
						close_section();
						open_section(filine);
					} else if (sectflag > 0) {
						parse_section(filine);
					}
					if (sectflag < 0) break;
				}
			}
			clin = 0;
			filine[0] = '\0';
			if (fcom == 1) fcom = 0;
		} else if (fcom == 0) {
			if (scur < 0x20 || scur > 0x7E) scur = ' ';
			if (scur == '#') {
				fcom = 1;
			} else if (sprev == '/' && scur == '/') {
				fcom = 1;
				if (clin > 0) clin--;
			} else if (sprev == '/' && scur == '*') {
				fcom = 2;
				if (clin > 0) clin--;
			} else if (clin < STR_LINE_SIZE-1) {
				if (scur == ' ') {
					if (filine[0] != '\0') {
						filine[clin] = scur;
						clin++;
					}
				} else {
					filine[clin] = scur;
					clin++;
				}
			}
		} else if (fcom == 2) {
			if (sprev == '*' && scur == '/') fcom = 0;
		}
		if (buf == EOF) break;
	}
	close_section();
	fclose(filpds);
	undefined_sections();
}

#endif
