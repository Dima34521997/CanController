// *** The application version. ***
// All modules major and minor versions must be equal to the listed ones
// and releases not less then the listed ones.

#define MAJ_VERS_APPL		1	// Major application software version
#define MIN_VERS_APPL		1	// Minor application software version
#define RELEASE_APPL		0	// The application software release

#define CAN_TIMERUSEC				10000	// CANopen timer period in microseconds (10 milliseconds).

#define CAN_TIMEOUT_RETRIEVE		500000	// SDO client BASIC transaction data retrieve DEFAULT timeout - microseconds
#define CAN_TIMEOUT_READ			200000	// SDO client BASIC transaction data read timeout - microseconds
											// NO nested client transactions

#define CAN_NOF_PDO_RECV_SLAVE		4		// No of receive PDO communication parameters for the Slave
#define CAN_NOF_PDO_TRAN_SLAVE		4		// No of transmit PDO communication parameters for the Slave

#define CAN_NOF_SYNCPDO_MASTER		64		// Synchronous RPDO and TPDO buffer sizes for the Master, 2.3

#define CAN_RPDO_TRTYPE		    	255		// Receive PDO default transmission type
#define CAN_RPDO_ET_MS		    	0		// RPDO event timer default time in ms

#define CAN_TPDO_TRTYPE			    255		// Transmit PDO default transmission type
#define CAN_TPDO_INHIBIT_100MCS	    0		// TPDO inhibit default time in hundreds of microseconds
#define CAN_TPDO_ET_MS			    0		// TPDO event timer default time in ms
#define CAN_TPDO_SYNC_START			0		// TPDO SYNC start value
