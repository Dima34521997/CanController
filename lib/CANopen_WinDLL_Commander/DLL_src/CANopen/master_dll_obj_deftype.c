#include <master_dll_header.h>

#if CHECK_VERSION_CANLIB(3, 0, 0)
//  CHECK_VERSION_CANLIB(2, 3, 1)

int16 get_deftype_bytes_objsize(canindex index, cansubind subind)
{
	if (index <= CAN_DEFTYPE_BOOLEAN || index > CAN_DEFTYPE_UNSIGNED32) return CAN_ERRET_OBD_NOOBJECT;
	if (subind != 0)  return CAN_ERRET_OBD_NOSUBIND;
	if (index == CAN_DEFTYPE_INTEGER16 || index == CAN_DEFTYPE_UNSIGNED16) return sizeof(unsigned16);
	else if (index == CAN_DEFTYPE_INTEGER32 || index == CAN_DEFTYPE_UNSIGNED32) return sizeof(unsigned32);
	return sizeof(unsigned8);
}

int16 see_deftype_access(canindex index, cansubind subind)
{
	int16 size;

	size = get_deftype_bytes_objsize(index, subind);
	if (size <= 0) return size;
	return CAN_MASK_ACCESS_PDO | CAN_MASK_ACCESS_RW;
}

int16 get_deftype_objtype(canindex index, cansubind subind)
{
	int16 size;

	size = get_deftype_bytes_objsize(index, subind);
	if (size <= 0) return size;
	return index;
}

int16 read_deftype_objdict(canindex index, cansubind subind, canbyte *data)
{
	int16 size, cnt;

	size = get_deftype_bytes_objsize(index, subind);
	if (size <= 0) return size;
	for (cnt = 0; cnt < size; cnt++) {
		*data = 0; data++;
	}
	return CAN_RETOK;
}

int16 write_deftype_objdict(canindex index, cansubind subind, canbyte *data)
{
	int16 size;

	size = get_deftype_bytes_objsize(index, subind);
	if (size <= 0) return size;
	return CAN_RETOK;
}

#endif
