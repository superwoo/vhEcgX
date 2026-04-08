//
//  BLEManager.cs
//  vhEcgX
//
//  Converted from Objective-C to C#
//  Copyright © 2018 谷山丰. All rights reserved.
//

using System;
using System.Collections.Generic;
using Foundation;
using CoreBluetooth;

namespace vhEcgX
{
    public interface IBLEManagerDelegate
    {
        void BleManagerDidDiscoverPeripheral(List<CBPeripheral> peripheralArray);
        void BleManagerDidConnectSuccessPeripheral();
    }

    public class BLEManager : NSObject, ICBCentralManagerDelegate, ICBPeripheralDelegate
    {
        private const string BLE_SERVICE_UUID = "6E400001-B5A3-F393-E0A9-E50E24DCCA9E";
        private const string BLE_UART_RX_UUID = "6E400002-B5A3-F393-E0A9-E50E24DCCA9E";
        private const string BLE_UART_TX_UUID = "6E400003-B5A3-F393-E0A9-E50E24DCCA9E";

        private static BLEManager instance;
        private static readonly object padlock = new object();
        private static CLowpassFilter2 lowPass;

        // 系统蓝牙设备管理对象，可以把他理解为主设备，通过他，可以去扫描和链接外设
        public CBCentralManager BleCentralManager { get; set; }

        // 当前链接的设备
        public CBPeripheral ConnectPeripheral { get; set; }

        // 保存扫描到的设备
        public List<CBPeripheral> PeripheralArray { get; set; }

        // 连接的特征值
        public CBCharacteristic UartTxCharacteristic { get; set; }

        // 读取数据的数组
        public List<NSNumber> ReciveDataArray { get; set; }

        // 当前外设是否已经连接
        public bool IsConnectPer { get; set; }

        public IBLEManagerDelegate Delegate { get; set; }

        private CBUUID serviceUUID;
        private CBUUID uartRxUUID;
        private CBUUID uartTxUUID;

        public CBUUID SERVICE_UUID
        {
            get
            {
                if (serviceUUID == null)
                    serviceUUID = CBUUID.FromString(BLE_SERVICE_UUID);
                return serviceUUID;
            }
        }

        public CBUUID UART_RX_UUID
        {
            get
            {
                if (uartRxUUID == null)
                    uartRxUUID = CBUUID.FromString(BLE_UART_RX_UUID);
                return uartRxUUID;
            }
        }

        public CBUUID UART_TX_UUID
        {
            get
            {
                if (uartTxUUID == null)
                    uartTxUUID = CBUUID.FromString(BLE_UART_TX_UUID);
                return uartTxUUID;
            }
        }

        public static BLEManager SharedInstance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new BLEManager();
                    }
                    return instance;
                }
            }
        }

        private BLEManager()
        {
            BleCentralManager = new CBCentralManager(this, null);
            PeripheralArray = new List<CBPeripheral>();
            ReciveDataArray = new List<NSNumber>();
            lowPass = new CLowpassFilter2(40, 500);
        }

        // 开始扫描外设
        public void StartScanPeriperals()
        {
            BleCentralManager.ScanForPeripherals((CBUUID[])null, null);
        }

        // 停止扫描
        public void StopScan()
        {
            BleCentralManager.StopScan();
        }

        // 连接外设
        public void ConnectToPeripheral()
        {
            BleCentralManager.ConnectPeripheral(ConnectPeripheral, null);
        }

        // 断开连接
        public void CancelConnectWithPeripheral()
        {
            StopScan();
            BleCentralManager.CancelPeripheralConnection(ConnectPeripheral);
            IsConnectPer = false;
        }

        public void StopReadDataWithCharacteristic()
        {
            ConnectPeripheral.SetNotifyValue(false, UartTxCharacteristic);
            CancelConnectWithPeripheral();
        }

        // 读取信号量
        // read RSSI
        public void ReadRSSIWithPeriperal()
        {
            ConnectPeripheral.ReadRSSI();
        }

        #region CBCentralManagerDelegate

        public void UpdatedState(CBCentralManager central)
        {
            if (central.State != CBCentralManagerState.PoweredOn)
            {
                Console.WriteLine("CBCentralManagerStatePoweredOff");
                return;
            }
        }

        public void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral,
            NSDictionary advertisementData, NSNumber RSSI)
        {
            Console.WriteLine($"didDiscoverPeripheral {peripheral} advertisementData=={advertisementData}");
            Console.WriteLine($"serviceUUID=={advertisementData["kCBAdvDataServiceUUIDs"]}");

            if (!PeripheralArray.Contains(peripheral))
            {
                PeripheralArray.Add(peripheral);
            }

            if (Delegate != null)
            {
                Delegate.BleManagerDidDiscoverPeripheral(PeripheralArray);
            }
        }

        public void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            peripheral.Delegate = this;
            peripheral.DiscoverServices(new CBUUID[] { SERVICE_UUID });
            Console.WriteLine("连接成功");

            if (ConnectPeripheral == peripheral)
            {
                if (!IsConnectPer)
                {
                    IsConnectPer = true;
                    if (Delegate != null)
                    {
                        Delegate.BleManagerDidConnectSuccessPeripheral();
                    }
                }
            }
        }

        public void FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Console.WriteLine($"连接失败， {error?.Description}");

            if (ConnectPeripheral == peripheral)
            {
                BleCentralManager.ConnectPeripheral(peripheral, null);
            }
        }

        public void DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            // 断开连接
            IsConnectPer = false;
        }

        #endregion

        #region CBPeripheralDelegate

        public void RssiRead(CBPeripheral peripheral, NSNumber RSSI, NSError error)
        {
            Console.WriteLine($"RSSI =={RSSI}");
        }

        public void DiscoveredService(CBPeripheral peripheral, NSError error)
        {
            foreach (var service in peripheral.Services)
            {
                if (service.UUID.Equals(SERVICE_UUID))
                {
                    peripheral.DiscoverCharacteristics(new CBUUID[] { UART_TX_UUID, UART_RX_UUID }, service);
                }
            }
            Console.WriteLine($"didDiscoverServices =={peripheral.Services}");
        }

        public void DiscoveredCharacteristic(CBPeripheral peripheral, CBService service, NSError error)
        {
            foreach (var characteristic in service.Characteristics)
            {
                peripheral.ReadValue(characteristic);
                if (characteristic.UUID.Equals(UART_TX_UUID))
                {
                    UartTxCharacteristic = characteristic;
                    peripheral.SetNotifyValue(true, characteristic);
                }
            }
            Console.WriteLine($"didDiscoverCharacteristicsForService =={service.Characteristics}");
        }

        public void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
        {
            Console.WriteLine($"didUpdateValueForCharacteristic ==={characteristic.Value}");
            DidReceiveData(characteristic.Value);
        }

        #endregion

        /*
         /// ADC增益 6倍
         const float ADC_GAIN = 4f;
         /// 1个ADC数据单位对应的uV值
         /// </summary>
         const float EMG_uV = (0x07fffff + 1) / 0x3A / VREF;
         /// 参考电压正输入 uV
         /// </summary>
         const float VREFP = 2.4f * 1000000;//uV
         /// <summary>
         /// 参考电压负输入 uV
         /// </summary>
         const float VREFN = 0f * 1000000;//uV
         /// <summary>
         /// 参考电压
         /// </summary>
         const float VREF = VREFP - VREFN;
         AdcData_uV = (int)(AdcData / ADC_GAIN / EMG_uV);//uv
         */

        private void DidReceiveData(NSData data)
        {
            int length = (int)data.Length;
            byte[] bytes = new byte[length];
            System.Runtime.InteropServices.Marshal.Copy(data.Bytes, bytes, 0, length);

            float VREFN = 0.0f * 1000000; //uV
            float VREFP = 2.4f * 1000000; //uV
            float VREF = VREFP - VREFN;

            float EMG_uV = (0x07fffff + 1) / VREF;
            float ADC_GAIN = 6.0f;

            for (int i = 1; i < length - 1; i = i + 3)
            {
                byte b1 = bytes[i];
                byte b2 = bytes[i + 1];
                byte b3 = bytes[i + 2];
                int numberInt = b1 << 24 | b2 << 16 | b3;
                int numberInt1 = numberInt >> 8;
                int AdcData_uV = (int)(numberInt1 / ADC_GAIN / EMG_uV);
                Console.WriteLine($"AdcData_uV==={AdcData_uV}");

                if (lowPass != null)
                {
                    lowPass.Filter(AdcData_uV); // 低通
                }

                Console.WriteLine($"AdcData_uV==={AdcData_uV}");

                NSNumber number = NSNumber.FromInt32(AdcData_uV);
                ReciveDataArray.Add(number);
            }
            Console.WriteLine($"didReceiveUARTData reciveDataArray length==={ReciveDataArray.Count}");
        }

        private void DidReceiveUARTData(NSData data)
        {
            int length = (int)data.Length;
            byte[] bytes = new byte[length];
            System.Runtime.InteropServices.Marshal.Copy(data.Bytes, bytes, 0, length);
            ReciveDataArray.Clear();

            for (int i = 1; i < length - 1; i = i + 3)
            {
                byte b1 = bytes[i];
                byte b2 = bytes[i + 1];
                byte b3 = bytes[i + 2];
                int numberInt = b1 * 65535 + b2 * 256 + b3;
                NSNumber number = NSNumber.FromInt32(numberInt - 16777216);
                ReciveDataArray.Add(number);
            }
            Console.WriteLine($"didReceiveUARTData reciveDataArray length==={ReciveDataArray.Count}");
            Console.WriteLine($"didReceiveUARTData reciveDataArray==={ReciveDataArray}");
        }
    }

    // Placeholder for CFilters class - needs to be converted from C++ to C#
    public class CLowpassFilter2
    {
        public CLowpassFilter2(int param1, int param2)
        {
            // Constructor implementation needed
        }

        public void Filter(int value)
        {
            // Filter implementation needed
        }
    }
}
