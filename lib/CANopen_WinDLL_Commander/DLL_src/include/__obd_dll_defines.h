#define OBD_NODES_SIZE			(2000+2)	// 8K max, byte granularity PROs (127*(4+4)*8)
                                      // +2 key end records

#define K1_LESS_K2    -1  // Keys comparison result: K1 < K2
#define K1_EQUAL_K2    0  // Keys comparison result: K1 = K2
#define K1_MORE_K2     1  // Keys comparison result: K1 > K2

#define CAN_NET_ACCESS_READ     1  // Read from network object
#define CAN_NET_ACCESS_WRITE    2  // Write from network object
