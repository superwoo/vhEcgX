//
//  HomeViewController.cs
//  vhEcgX
//
//  Converted from Objective-C to C#
//  Copyright © 2018 谷山丰. All rights reserved.
//

using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CoreBluetooth;

namespace vhEcgX
{
    public class HomeViewController : UIViewController, IUITableViewDelegate, IUITableViewDataSource, IBLEManagerDelegate
    {
        private UITableView bleTableView;
        public List<CBPeripheral> BleDeviceList { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.Red;
            Title = "BLE Device";
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("扫描", UIBarButtonItemStyle.Plain, ScanBleDevice);
            BleDeviceList = new List<CBPeripheral>();
            SetupUI();
            BLEManager.SharedInstance.Delegate = this;
        }

        private void ScanBleDevice(object sender, EventArgs e)
        {
            BLEManager.SharedInstance.StartScanPeriperals();
        }

        private void SetupUI()
        {
            bleTableView = new UITableView();
            View.AddSubview(bleTableView);
            bleTableView.BackgroundColor = UIColor.White;
            bleTableView.Delegate = this;
            bleTableView.DataSource = this;

            // Using Masonry equivalent in C# - would need Cirrious.FluentLayout or similar
            // For now, using frame-based layout
            bleTableView.Frame = View.Bounds;
            bleTableView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
        }

        #region IBLEManagerDelegate

        public void BleManagerDidDiscoverPeripheral(List<CBPeripheral> peripheralArray)
        {
            BleDeviceList = BLEManager.SharedInstance.PeripheralArray;
            bleTableView.ReloadData();
        }

        public void BleManagerDidConnectSuccessPeripheral()
        {
            BLEManager.SharedInstance.StopScan();
            var ctr = new CollectDataViewController();
            NavigationController.PushViewController(ctr, true);
        }

        #endregion

        #region UITableView DataSource and Delegate

        [Export("tableView:heightForRowAtIndexPath:")]
        public nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 80;
        }

        [Export("tableView:numberOfRowsInSection:")]
        public nint RowsInSection(UITableView tableView, nint section)
        {
            return BleDeviceList.Count;
        }

        [Export("tableView:cellForRowAtIndexPath:")]
        public UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            string cellID = "cellID";
            var cell = tableView.DequeueReusableCell(cellID);
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellID);
                cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
            }

            var peripheral = BleDeviceList[indexPath.Row];
            cell.TextLabel.Text = peripheral.Name;
            return cell;
        }

        [Export("tableView:didSelectRowAtIndexPath:")]
        public void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, false);
            var per = BleDeviceList[indexPath.Row];
            BLEManager.SharedInstance.ConnectPeripheral = per;
            BLEManager.SharedInstance.ConnectToPeripheral();
            BLEManager.SharedInstance.ReciveDataArray.Clear();
        }

        #endregion
    }
}
