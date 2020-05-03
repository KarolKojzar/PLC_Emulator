using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PLCEmulator
{
    enum errorCodes
    {
        ERR_REG_WND_CLASS = 1,
        ERR_CREATE_WINDOW = 2,
        ERR_REG_DEV_NOTIFY = 3,
        ERR_GET_MESSAGE = 4,
        ERR_CREATE_THR = 5,
        ERR_ALLREADY_CONNECTED = 6,
        ERR_DEVICE_DISCONNECTED = 7,
        ERR_NO_DEVICES_TO_CONNECT = 8,
        ERR_UNABLE_TO_LOAD_USBAPI_LIBRARY = 9,
        ERR_USB_OPEN = 10,
        ERR_TX = 11,
        ERR_RX = 12,
        ERR_CONN_CONFIG = 13,
        ERR_WRONG_FIRMWARE = 14,
        ERR_DEVICE_INIT = 15,
        ERR_MEM_ALLOC = 16,
        ERR_UNABLE_TO_START_TIMER = 17,
        ERR_NONE = 0
    };

    enum stateCodes
    {
        STATE_DISCONNECTED = 0, 
        STATE_CONNECTED = 1,        
        STATE_ERROR = 2
    };

    enum aiTypes
    {
        AI_VOLTAGE = 0x00,
        AI_CURRENT = 0x01,
    };

    class USBComm
    {


      
        /* Device handling functions */


        [StructLayout(LayoutKind.Sequential)]
        public struct TDeviceState
        {
            public int state;
            public int errorStatus;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct TFirmwareVersion
        {
            public char major;
            public char minor;
        };


        /*
         * usbConnect() - should be ran before any other operation.
         * On success returns ERR_NONE, on any other error ERR_XXX, 
         * where XXX is one of the defined error codes in errorCodes enum
         */
        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbConnect();
        /*
         * usbDisconnect() - should be ran on program close
         */
        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbDisconnect();

        /*
         * usbGetDeviceSerialNumber()
         * Currently not implemented.
         */
        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetDeviceSerialNumber();

        /*
         * usbGetDeviceVersion()
         * Currently not implemented, checked in DLL for correctness.
         */
        [DllImport("PLCSigGenComm.dll")]
        public static extern TFirmwareVersion usbGetDeviceVersion();


        /*
         * usbGetDeviceState()
         * Returns the TDeviceState structure with device state and an eventual
         * error code. If an error is reported the combination od usbDisconnect(); usbConnect();
         * should be used/
         */
        [DllImport("PLCSigGenComm.dll")]
        public static extern TDeviceState usbGetDeviceState();


        /*
         * usbResetDevice()
         * Currently not implemented. Use the previously shown sequence.
         */
        [DllImport("PLCSigGenComm.dll")]
        public static extern void usbResetDevice();

        /* Get the number of digital and analog inputs/outputs */



        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetNumDI();

        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetNumDO();

        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetNumAI();

        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetNumAO();


        /* Digital inputs and outputs
         * For inputs all 16 channels are permited to read.
         * All 16 output channels can be written, but the last 4 channels depend on
         * their configuration. Currently encoder simulation is not implemented. 
         * We are waiting for Greg to finish programming the firmware.
         */
        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetDI(int num);

        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetAllDI(int[] data);

        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbSetDO(int num, int value);

        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetDO(int num);

        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetAllDO(int[] data);


        // Analog inputs and outputs 

        [DllImport("PLCSigGenComm.dll")]
        public static extern short usbGetAI(int num);

        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbGetAllAI(short[] data);

        [DllImport("PLCSigGenComm.dll")]
        public static extern int usbSetAO(int num, short value);

        [DllImport("PLCSigGenComm.dll")]
        public static extern short usbGetAO(int num);

        [DllImport("PLCSigGenComm.dll")]
        public static extern void usbGetAllAO(short[] data);


        /* Encoder simulation */
        [DllImport("PLCSigGenComm.dll")]
        public static extern void usbSetEncoder (short speed,char dir);
        [DllImport("PLCSigGenComm.dll")]
        public static extern void usbDisEncoder();


    }
}
