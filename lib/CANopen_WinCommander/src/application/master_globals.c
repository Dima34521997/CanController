#include <master_header.h>

unsigned8 	can_network;	// CAN network
unsigned8	bitrate_index; 	// Bit rate index

struct canopennode can_node[CAN_NOF_NODES+1];

char log_file_name[STR_FILE_NAME_SIZE];		// Log file name
