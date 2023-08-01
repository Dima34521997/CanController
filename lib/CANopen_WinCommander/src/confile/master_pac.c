#include <master_header.h>

#if CHECK_VERSION_APPL(1, 1, 2)

int16 find_token(char *dtok, char *data, unsigned16 *npos)
{
	while (*dtok != '\0') {
		if (*dtok != toupper(*data)) return GEN_ERRET_TOKEN;
		dtok++; data++; (*npos)++;
	}
	return GEN_RETOK;
}

int16 parse_float(union numbers *num, char *data, unsigned16 dtype, unsigned16 *npos)
{
	real64 result, fract, sign;
	int16 fldot;

	num->re64 = 0.0;
	while (*data != '-' && (*data < '0' || *data > '9')) {
		if (*data == '\0') return GEN_ERRET_EMPTY;
		data++; (*npos)++;
	}
	result = 0.0; fract = 0.1; fldot = 0; sign = 1.0;
	if (*data == '-') {
		sign = -1.0;
		data++; (*npos)++;
	}
	while (TRUE) {
		if (*data == '.') {
			if (fldot == 0) fldot = 1;
			else break;
		} else if (*data >= '0' && *data <= '9') {
			if (fldot == 0) {
				result = result*10.0 + (*data - '0')*1.0;
			} else {
				result = result + (*data - '0')*fract;
				fract *= 0.1;
			}
		} else break;
		data++; (*npos)++;
	}
	if (dtype == CAN_DEFTYPE_REAL32) num->re32 = (float)(sign * result);
	else if (dtype == CAN_DEFTYPE_REAL64) num->re64 = sign * result;
	else return GEN_ERRET_DATATYPE;
	return GEN_RETOK;
}

int16 parse_integer(union numbers *num, char *data, unsigned16 dtype, int16 base, unsigned16 *npos)
{
	int16 sign, fig;
	unsigned64 result;
	int64 resign;

	num->init = 0;
	while (*data != '-' && (*data < '0' || *data > '9') && (toupper(*data) < 'A' || toupper(*data) > 'Z')) {
		if (*data == '\0') return GEN_ERRET_EMPTY;
		data++; (*npos)++;
	}
	if (base == 0) {
		if (*data == '0') {
			data++; (*npos)++;
			if (toupper(*data) == 'X') {
				base = 16;
				data++; (*npos)++;
			} else base = 8;
		} else base = 10;
	} else if (base < 2 || base > 36)  return GEN_ERRET;
	result = 0; sign = 1;
	if (*data == '-') {
		sign = -1;
		data++; (*npos)++;
	}
	while (TRUE) {
		if (base > 10 && toupper(*data) >= 'A' && toupper(*data) <= 'Z') fig = 10 + toupper(*data) - 'A';
		else fig = *data - '0';
		if (fig >= 0 && fig < base) result = result*base + fig;
		else break;
		data++; (*npos)++;
	}
	resign = sign * result;
	if (dtype == CAN_DEFTYPE_INTEGER8) {
		num->i8 = (int8)resign;
		if (num->i8 != resign) return GEN_ERRET_VALUE;
	} else if (dtype == CAN_DEFTYPE_UNSIGNED8) {
		num->uns8 = (unsigned8)result;
		if (num->uns8 != resign) return GEN_ERRET_VALUE;
	} else if (dtype == CAN_DEFTYPE_INTEGER16) {
		num->i16 = (int16)resign;
		if (num->i16 != resign) return GEN_ERRET_VALUE;
	} else if (dtype == CAN_DEFTYPE_UNSIGNED16) {
		num->uns16 = (unsigned16)result;
		if (num->uns16 != resign) return GEN_ERRET_VALUE;
	} else if (dtype == CAN_DEFTYPE_INTEGER24 || dtype == CAN_DEFTYPE_INTEGER32) {
		num->i32 = (int32)resign;
		if (num->i32 != resign) return GEN_ERRET_VALUE;
	} else if (dtype == CAN_DEFTYPE_UNSIGNED24 || dtype == CAN_DEFTYPE_UNSIGNED32) {
		num->uns32 = (unsigned32)result;
		if (num->uns32 != resign) return GEN_ERRET_VALUE;
	} else if (dtype == CAN_DEFTYPE_INTEGER40 || dtype == CAN_DEFTYPE_INTEGER48 ||
		dtype == CAN_DEFTYPE_INTEGER56 || dtype == CAN_DEFTYPE_INTEGER64) {
		num->i64 = resign;
	} else if (dtype == CAN_DEFTYPE_UNSIGNED40 || dtype == CAN_DEFTYPE_UNSIGNED48 ||
		dtype == CAN_DEFTYPE_UNSIGNED56 || dtype == CAN_DEFTYPE_UNSIGNED64) {
		num->uns64 = result;
	} else return GEN_ERRET_DATATYPE;
	return GEN_RETOK;
}

int16 parse_number(union numbers *num, char *data, unsigned16 dtype, unsigned16 *npos)
{
	if (dtype == CAN_DEFTYPE_BOOLEAN) {
		num->init = 0;
		while (toupper(*data) != 'T' && toupper(*data) != 'F') {
			if (*data == '\0') return GEN_ERRET_EMPTY;
			data++;  (*npos)++;
		}
		(*npos)++;
		if (toupper(*data) == 'F') num->uns8 = FALSE;
		else num->uns8 = TRUE;
		return GEN_RETOK;
	}
	if (dtype == CAN_DEFTYPE_REAL32 || dtype == CAN_DEFTYPE_REAL64) {
		return parse_float(num, data, dtype, npos);
	}
	return parse_integer(num, data, dtype, 0, npos);
}

int16 parse_string(char *dest, char *src, char ignore, int16 strlen, unsigned16 *npos)	// 1.1.1 API changed
{
	while (*src <= 0x20 || *src > 0x7E || *src == ignore) {
		if (*src == '\0') {
			*dest = '\0';
			return GEN_ERRET_EMPTY;
		}
		src++; (*npos)++;
	}
	if (strlen <= 0) strlen = 0;
	else strlen--;
	while (strlen > 0) {
		*dest = *src;
		if (*dest < 0x20 || *dest > 0x7E) *dest = ' ';
		src++; (*npos)++;
		dest++; strlen--;
		if (*src == '\0') break;
	}
	while (strlen >= 0) {
		*dest = '\0';
		dest++; strlen--;
	}
	return GEN_RETOK;
}

void clip_string(char *src, int16 strlen)
{
	if (strlen <= 0) return;
	strlen--;
	src += strlen;
	while (strlen >= 0) {
		if (*src <= 0x20 || *src > 0x7E) *src = '\0';
		else break;
		src--;
		strlen--;
	}
}

#endif
