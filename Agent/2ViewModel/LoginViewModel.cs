﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using EasyWpfLoginNavigateExample.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using EasyWpfLoginNavigateExample.Helpers;
using MyDeviceTest;
using RAT.ZTry;

namespace Agent._2ViewModel
{
    class LoginViewModel : ViewModelBase
    {
        AzureLoginService azureService;
        public LoginViewModel()
        {
            azureService = new AzureLoginService();
            LoginCommand = new RelayCommand(DoLogin);
            LogOutCommand = new RelayCommand(DoLogout);
            MonitorCommand = new RelayCommand(DoMonitor);
            myDeviceTest = new DeviceTest();
        }

        private DeviceTest myDeviceTest;
        #region Members
        //Login validation command
        public RelayCommand LoginCommand { get; set; }
        public RelayCommand LogOutCommand { get; set; }
        public RelayCommand MonitorCommand { get; set; }

        //Login details
        private string userName = "";
        private string password = "";

        #endregion

        #region Properties

        public string UserName
        {
            get { return userName; }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }

        #endregion

        private void DoLogin(object obj)
        {
            //Getting Password
            var passwordBox = obj as PasswordBox;
            var password = passwordBox.Password;
            this.password = password.ToString();

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void DoLogout(object obj)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_Logout;
            worker.RunWorkerAsync();

        }
        private void DoMonitor(object obj)
        {
            buttonName();
        }

        private bool monitor = false;

        public void buttonName()
        {
            if (monitor)
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).button2.Content = "Off";
                monitor = false;
                myDeviceTest.stop();
                System.Diagnostics.Debug.WriteLine("\n-----STOP");
            }
            else
            {
                ((MainWindow)System.Windows.Application.Current.MainWindow).button2.Content = "On";
                monitor = true;
                System.Diagnostics.Debug.WriteLine("\n-----Level 1");
                myDeviceTest.Test();
                System.Diagnostics.Debug.WriteLine("\n-----Start");
            }
        }
        void worker_ssss(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(buttonName, DispatcherPriority.ContextIdle);
        }

        void worker_Logout(object sender, DoWorkEventArgs e)
        {
            //int max = (int)e.Argument;
            int result = 0;

            Application.Current.Dispatcher.Invoke(Logout, DispatcherPriority.ContextIdle);
        }


        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            LoginValidate();
        }

        public void hideElements()
        {
            ((MainWindow) System.Windows.Application.Current.MainWindow).label.Visibility = Visibility.Collapsed;
            ((MainWindow) System.Windows.Application.Current.MainWindow).label1.Visibility = Visibility.Collapsed;
            ((MainWindow) System.Windows.Application.Current.MainWindow).button.Visibility = Visibility.Collapsed;
            ((MainWindow) System.Windows.Application.Current.MainWindow).textBox.Visibility = Visibility.Collapsed;
            ((MainWindow) System.Windows.Application.Current.MainWindow).passwordBox.Visibility = Visibility.Collapsed;
            //Resetting Text Boxes
            ((MainWindow) System.Windows.Application.Current.MainWindow).textBox.Text = "";
            ((MainWindow) System.Windows.Application.Current.MainWindow).passwordBox.Password = "";
            //Show
            ((MainWindow) System.Windows.Application.Current.MainWindow).button2.Visibility = Visibility.Visible;
            ((MainWindow) System.Windows.Application.Current.MainWindow).button3.Visibility = Visibility.Visible;
            ((MainWindow) System.Windows.Application.Current.MainWindow).label3.Visibility = Visibility.Visible;
            ((MainWindow) System.Windows.Application.Current.MainWindow).label4.Visibility = Visibility.Visible;
            ((MainWindow) System.Windows.Application.Current.MainWindow).list.Visibility = Visibility.Visible;
        }

        public void Logout()
        {
            ((MainWindow) System.Windows.Application.Current.MainWindow).label.Visibility = Visibility.Visible;
            ((MainWindow) System.Windows.Application.Current.MainWindow).label1.Visibility = Visibility.Visible;
            ((MainWindow) System.Windows.Application.Current.MainWindow).button.Visibility = Visibility.Visible;
            ((MainWindow) System.Windows.Application.Current.MainWindow).textBox.Visibility = Visibility.Visible;
            ((MainWindow) System.Windows.Application.Current.MainWindow).passwordBox.Visibility = Visibility.Visible;
            //Show
            ((MainWindow) System.Windows.Application.Current.MainWindow).button2.Visibility = Visibility.Collapsed;
            ((MainWindow) System.Windows.Application.Current.MainWindow).button3.Visibility = Visibility.Collapsed;
            ((MainWindow) System.Windows.Application.Current.MainWindow).label3.Visibility = Visibility.Collapsed;
            ((MainWindow) System.Windows.Application.Current.MainWindow).label4.Visibility = Visibility.Collapsed;
            ((MainWindow) System.Windows.Application.Current.MainWindow).list.Visibility = Visibility.Collapsed;

        }

        //Checks Login, Changes screen if found
        async void LoginValidate(){
           if (await azureService.GetLogin(UserName, password))
            {
                //Screen Navigation
                Application.Current.Dispatcher.Invoke(hideElements, DispatcherPriority.ContextIdle);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Incorrect Login");
                string messageBoxText = "Login failed";
                string caption = "Word Processor";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
    
                // Display message box
            MessageBox.Show(messageBoxText, caption, button, icon);
            }
        }
    }
}
