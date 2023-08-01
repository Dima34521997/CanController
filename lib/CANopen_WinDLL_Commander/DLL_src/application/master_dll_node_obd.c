#include <master_dll_header.h>

#if CHECK_VERSION_APPL(1, 1, 0)
//  CHECK_VERSION_APPL(1, 0, 0)

static int32 dictfill;
static struct obdentry dict[OBD_NODES_SIZE];


static unsigned8 get_object_size(canindex type)
{
	if (type == CAN_DEFTYPE_INTEGER8) return 1;
	if (type == CAN_DEFTYPE_UNSIGNED8) return 1;
	if (type == CAN_DEFTYPE_INTEGER16) return 2;
	if (type == CAN_DEFTYPE_UNSIGNED16) return 2;
	if (type == CAN_DEFTYPE_INTEGER32) return 4;
	if (type == CAN_DEFTYPE_UNSIGNED32) return 4;
	if (type == CAN_DEFTYPE_INTEGER64) return 8;
	if (type == CAN_DEFTYPE_UNSIGNED64) return 8;
	if (type == CAN_DEFTYPE_REAL32) return 4;
	if (type == CAN_DEFTYPE_REAL64) return 8;
	return 0;
}

static int16 key_obd(struct obdentry *obd1, struct obdentry *obd2)
{
	if (obd1->node < obd2->node) return K1_LESS_K2;
	if (obd1->node > obd2->node) return K1_MORE_K2;
	if (obd1->index < obd2->index) return K1_LESS_K2;
	if (obd1->index > obd2->index) return K1_MORE_K2;
	if (obd1->subind < obd2->subind) return K1_LESS_K2;
	if (obd1->subind > obd2->subind) return K1_MORE_K2;
	return K1_EQUAL_K2;
}

static int16 add_node_object(struct obdentry *obj)
{
	int32 first, last, med, cnt;
	int16 cmp;

	if (dictfill+1 >= OBD_NODES_SIZE) return GEN_ERRET_OUTOFMEM;
	first = 0;
	last = dictfill;
	while (last - first > 1) {
		med = (first + last) / 2;
		cmp = key_obd(obj, &dict[med]);
		if (cmp == K1_LESS_K2) last = med;
		else if (cmp == K1_MORE_K2) first = med;
		else return GEN_ERRET_DUPLICATED;
	}
	dictfill++;
	for (cnt = dictfill; cnt > last; cnt--) {
		memcpy(&dict[cnt], &dict[cnt-1], sizeof(struct obdentry));
	}
	memcpy(&dict[last], obj, sizeof(struct obdentry));
	return GEN_RETOK;
}

int16 add_node_object_dictionary(cannode node, canindex index, cansubind subind, canindex type)
{
	struct obdentry obj;
	unsigned8 size;

	if (node < CAN_NODE_ID_MIN || node > CAN_NODE_ID_MAX) return CAN_ERRET_NODEID;
	size = get_object_size(type);
	if (size == 0) return GEN_ERRET_DATATYPE; 
	memset(&obj, 0, sizeof(obj));
	obj.node = node;
	obj.index = index;
	obj.subind = subind;
	obj.size = size;
	return add_node_object(&obj);
}

static int32 find_object(cannode node, canindex index, cansubind subind)
{
	int32 first, last, med;
	int16 cmp;
	struct obdentry obj;

	memset(&obj, 0, sizeof(obj));
	obj.node = node;
	obj.index = index;
	obj.subind = subind;
 	first = 0;
	last = dictfill;
	while (last - first > 1) {
		med = (first + last) / 2;
		cmp = key_obd(&obj, &dict[med]);
		if (cmp == K1_LESS_K2) last = med;
		else if (cmp == K1_MORE_K2) first = med;
		else return med;
	}
	return -1;
}

int32 node_get_manstan_objsize(cannode node, canindex index, cansubind subind)
{
	int32 obn;

	if (index >= CAN_INDEX_DEFTYPE_MIN && index <= CAN_INDEX_DEFTYPE_MAX) {
		return get_deftype_bytes_objsize(index, subind);
	}
	obn = find_object(node, index, subind);
	if (obn < 0) return CAN_ERRET_OBD_NOOBJECT;
	return dict[obn].size;
}

int16 node_read_manstan_objdict(cannode node, canindex index, cansubind subind, canbyte *data)
{
	int32 obn, cnt;

	if (index >= CAN_INDEX_DEFTYPE_MIN && index <= CAN_INDEX_DEFTYPE_MAX) {
		return read_deftype_objdict(index, subind, data);
	}
	obn = find_object(node, index, subind);
	if (obn < 0) return CAN_ERRET_OBD_NOOBJECT;
	CAN_CRITICAL_BEGIN
	for (cnt = 0; cnt < dict[obn].size; cnt++) {
		*data = dict[obn].bd[cnt];
		data++;
	}
	CAN_CRITICAL_END
	return CAN_RETOK;
}

int16 node_write_manstan_objdict(cannode node, canindex index, cansubind subind, canbyte *data)
{
	int32 obn, cnt;

	if (index >= CAN_INDEX_DEFTYPE_MIN && index <= CAN_INDEX_DEFTYPE_MAX) {
		return write_deftype_objdict(index, subind, data);
	}
	obn = find_object(node, index, subind);
	if (obn < 0) return CAN_ERRET_OBD_NOOBJECT;
	cnt = 0;
	CAN_CRITICAL_BEGIN
	while (cnt < dict[obn].size) {
		dict[obn].bd[cnt] = *data;
		data++; cnt++;
	}
	while (cnt < 8) {
		dict[obn].bd[cnt] = 0;
		cnt++;
	}
	dict[obn].updated = TRUE;
	CAN_CRITICAL_END
	return CAN_RETOK;
}

int16 read_node_object_dictionary(cannode node, canindex index, cansubind subind, unsigned8 *upd, union numbers *num)
{
	int32 obn, cnt;
	canbyte *bpnt;

	num->uns64 = 0;
	obn = find_object(node, index, subind);
	if (obn < 0) return CAN_ERRET_OBD_NOOBJECT;
	bpnt = (canbyte*)num;
	CAN_CRITICAL_BEGIN
	for (cnt = 0; cnt < dict[obn].size; cnt++) {
		*bpnt = dict[obn].bd[cnt];
		bpnt++;
	}
	*upd = dict[obn].updated;
	dict[obn].updated = FALSE;
	CAN_CRITICAL_END
	return CAN_RETOK;
}

int16 write_node_object_dictionary(cannode node, canindex index, cansubind subind, union numbers *num)
{
	int32 obn, cnt;
	canbyte *bpnt;

	obn = find_object(node, index, subind);
	if (obn < 0) return CAN_ERRET_OBD_NOOBJECT;
	bpnt = (canbyte*)num;
	cnt = 0;
	CAN_CRITICAL_BEGIN
	while (cnt < dict[obn].size) {
		dict[obn].bd[cnt] = *bpnt;
		cnt++; bpnt++;
	}
	while (cnt < 8) {
		dict[obn].bd[cnt] = 0;
		cnt++;
	}
	CAN_CRITICAL_END
	return CAN_RETOK;
}

int16 get_node_updated_object(cannode *node, canindex *index, cansubind *subind, union numbers *num)
{
	int32 obn, cnt;
	canbyte *bpnt;
	
	*node = 0;
	*index = 0;
	*subind = 0;
	num->uns64 = 0;
	bpnt = (canbyte*)num;
	for (obn = 1; obn < dictfill; obn++) {
		if (dict[obn].updated == TRUE) {
			CAN_CRITICAL_BEGIN
			*node = dict[obn].node;
			*index = dict[obn].index;
			*subind = dict[obn].subind;
			for (cnt = 0; cnt < dict[obn].size; cnt++) {
				*bpnt = dict[obn].bd[cnt];
				bpnt++;
			}
			dict[obn].updated = FALSE;
			CAN_CRITICAL_END
			return CAN_RETOK;
		}
	}
	return GEN_ERRET_NO_UPDATED;
}

void init_node_obd(void)
{
	unsigned16 cnt;

	for (cnt = 0; cnt < OBD_NODES_SIZE; cnt++) {
		memset(&dict[cnt], 0, sizeof(struct obdentry));
	}
	dict[1].node = 0xFF;
	dict[1].index = 0xFFFF;
	dict[1].subind = 0xFF;
	dictfill = 1;
}

#endif
