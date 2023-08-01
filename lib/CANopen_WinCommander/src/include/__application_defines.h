// *** The application version. ***
// All modules major and minor versions must be equal to the listed ones
// and releases not less then the listed ones.

#define MAJ_VERS_APPL		1	// Major application software version
#define MIN_VERS_APPL		1	// Minor application software version
#define RELEASE_APPL		0	// The application software release


#define CAN_NETWORK_CONTROLLER		0							// Default CAN driver channel (CAN controller number)
#define CAN_BITRATE_INDEX			CIA_BITRATE_INDEX_500		// Default CAN network bit rate index

#define CAN_TIMERUSEC				10000	// Background timer period in microseconds (10 milliseconds).

#define CAN_TIMEOUT_RETRIEVE		500000	// SDO client BASIC transaction data retrieve DEFAULT timeout - microseconds
#define CAN_TIMEOUT_READ			200000	// SDO client BASIC transaction data read timeout - microseconds
											// NO nested client transactions

#define CAN_HBT_PRODUCER_MS			1000	// Producer configured heartbeat time, ms
#define CAN_HBT_CONSUMER_MS			1200	// Consumer configured heartbeat time, ms

#define CAN_CONFIG_NODE_MS			5000	// Configure node timeout and reset if failed, ms
#define CAN_RESET_NODE_MS			10000	// Reset node period for non-responding node, ms

/*
#define CAN_RPDO_TRTYPE		    	0		// Receive PDO default transmission type
#define CAN_TPDO_TRTYPE			    0		// Transmit PDO default transmission type
#define CAN_TPDO_INHIBIT_100MCS	    0		// TPDO inhibit default time in hundreds of microseconds
#define CAN_RPDO_ET_MS		    	0		// RPDO event timer default time in ms
#define CAN_TPDO_ET_MS			    0		// TPDO event timer default time in ms
*/

#define MASTER_CONFIG_FILE_NAME		"CANopenMaster.cfg"
#define MASTER_CONFIG_FILE_VERSION	0x00030001

#define MASTER_LOG_FILE_NAME_DEF	"CANopenMaster.log"		// Default

#define CAN_NETWORK_MIN				0		// Min CAN controller channel
#define CAN_NETWORK_MAX				1		// Max CAN controller channel

#define STR_FILE_NAME_SIZE			256		// File names max. length, including path (255 and 0)
#define STR_LINE_SIZE				256		// Lines max. length (255 and 0)
#define STR_TS_SIZE					 20		// Time Stamp YYYY-MM-DD HH:MM:SS (19 and 0)
