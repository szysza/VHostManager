﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using MaxMind.GeoIP;
using Microsoft.Win32;

namespace VHostManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IList<string> _ipAdresses;
        private IList<string> _restults;
        private readonly LookupService _lService;
        private readonly LookupService _lServiceDomain;


        public MainWindow()
        {
            InitializeComponent();
            var projectPath = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
            _lService = new LookupService(projectPath + "/GeoIPDatabase/GeoLiteCity.dat", LookupService.GEOIP_STANDARD);
            _lServiceDomain = new LookupService(projectPath + "/GeoIPDatabase/GeoIPDomain.dat", LookupService.GEOIP_STANDARD);
            _restults = new List<string>();
        }

        private void ListCountries()
        {
            if (_ipAdresses.Count < 1)
                return;

            var sb = new StringBuilder("");

            foreach (var ip in _ipAdresses)
            {
                var location = _lService.getLocation(ip);
                var domain = _lServiceDomain.getOrg(ip);
                var domainTxt = domain != null ? ", Domena: " + domain : "";

                if (location == null)
                    continue;

                sb.Append("Adres IP: " + ip + ", kraj: " + location.countryName + domainTxt + "\n");
               

            }

            var lines = sb.ToString().Split('\n');
            for (var i = 0; i < lines.Length; i += 1)
            {
                lines[i] = lines[i].TrimEnd('\n');
                _restults.Add(lines[i]);
            }


            TxtResult.Text = sb.ToString();

        }

        private void wczytaj_btn_click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog {DefaultExt = ".txt", Filter = "TXT Files (*.txt)|*.txt"};

            var result = dlg.ShowDialog();


            if (result != true) 
                return;
           
            var filename = dlg.FileName;
            _ipAdresses = new List<string>();

            try
            {
                using (var sr = new StreamReader(filename))
                {
                    var line = sr.ReadToEnd();
                    
                    var lines = line.Split('\n');
                    for (var i = 0; i < lines.Length; i += 1)
                    {
                        lines[i] = lines[i].TrimEnd('\r');
                        _ipAdresses.Add(lines[i]);
                    }
                       

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(ex.Message);
            }

            ListCountries();
        }


        private void zapisz_btn_click(object sender, RoutedEventArgs e)
        {
            var saveFileDlg = new SaveFileDialog
            {
                DefaultExt = ".txt",
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*",
                RestoreDirectory = true,
                FileName = "result"
            };

           
            if (saveFileDlg.ShowDialog() == true && saveFileDlg.FileName != "")
            {
                var sb = new StringBuilder("");

                foreach (var line in _restults)
                {
                    sb.Append(line + Environment.NewLine);
                }

                File.WriteAllText(saveFileDlg.FileName, sb.ToString());
            }
        } 
    }
}
