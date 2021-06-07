using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InterlockingDemo.Classes;
using InterlockingDemo.Helpler;
using System.Collections.Generic;
using System.Windows;

namespace InterlockingDemo.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region <properties>
        public Structure structure;

        private string copyright_name;
        public string Copyright_name
        {
            get { return copyright_name; }
            set { copyright_name = value;RaisePropertyChanged(); }
        }

        private string copyright_univer;
        public string Copyright_univer
        {
            get { return copyright_univer; }
            set { copyright_univer = value; RaisePropertyChanged(); }
        }
        #endregion


        public MainViewModel()
        {
            InitProperties();
            InitCommands();
        }


        #region <commands>
        public RelayCommand SelectInterlockingFileCM { get; set; }
        public RelayCommand SelectSignalFileCM { get; set; }
        public RelayCommand SelectSwitchFileCM { get; set; }
        public RelayCommand SelectSectionFileCM { get; set; }
        public RelayCommand CheckInterlockingFeasibilityCM { get; set; }
        #endregion


        #region <functions>
        public void InitProperties()
        {
            structure = new Structure();
            Copyright_name = "—— by Pu Fan";
            Copyright_univer = "Beijing Jiaotong University";
        }

        public void InitCommands()
        {
            SelectInterlockingFileCM = new RelayCommand(SelectInterlockingFileFunc);
            SelectSignalFileCM = new RelayCommand(SelectSignalFileFunc);
            SelectSwitchFileCM = new RelayCommand(SelectSwitchFileFunc);
            SelectSectionFileCM = new RelayCommand(SelectSectionFileFunc);
            CheckInterlockingFeasibilityCM = new RelayCommand(CheckInterlockingFeasibilityFunc);
        }

        public void SelectInterlockingFileFunc()
        {
            structure.Routes = EXCELHelper.ReadRouteEXCELFile();
        }

        public void SelectSignalFileFunc()
        {
            structure.Signals = EXCELHelper.ReadSignalEXCELFile();
        }

        public void SelectSwitchFileFunc()
        {
            structure.Switches = EXCELHelper.ReadSwitchEXCELFile();
        }

        public void SelectSectionFileFunc()
        {
            structure.Sections = EXCELHelper.ReadSectionEXCELFile();
        }

        public void CheckInterlockingFeasibilityFunc()
        {            
            if (CheckDataIntegrility())
            {
                foreach(string route_key in structure.Routes.Keys)
                {
                    Route route = structure.Routes[route_key];
                    System.Type t = route.GetType();
                    //判断是列车进路还是调车进路
                    if (t == typeof(TrainRoute))
                    {
                        TrainRoute train_route = route as TrainRoute;
                        string direction = train_route.HeadToDirection;
                        string start_point = Route.FormateStringIntoEquipmentName(train_route.StartButton);

                        string end_point = train_route.EndButton;
                        if (end_point == "SLZA")
                        {
                            end_point = "D1";
                        }
                        else if (end_point == "XLZA")
                        {
                            end_point = "D2";
                        }
                        else if(end_point== "SⅠLA")
                        {
                            end_point = "D5";
                        }
                        else if(end_point== "XⅡLA")
                        {
                            end_point = "D10";
                        }
                        else
                        {
                            end_point = Route.FormateStringIntoEquipmentName(end_point);
                        }
                        List<List<string>> greedy_path = new List<List<string>>();
                        int index = 0;
                        Route.GenEquipmentString(structure, greedy_path, index, start_point, end_point, direction);
                        string switch_string = Route.GenSwitchString(structure, greedy_path);
                        train_route.GeneratedSwitchString = switch_string;
                    }
                    else if (t == typeof(ShuntRoute))
                    {
                        ShuntRoute shunt_route = route as ShuntRoute;
                        string direction = shunt_route.HeadToDirection;
                        string start_point = Route.FormateStringIntoEquipmentName(shunt_route.StartButton);
                        string end_point = shunt_route.EndButton;
                        if (end_point == "SⅠDA")
                        {
                            end_point = "D5";
                        }
                        else if (end_point == "XⅡDA")
                        {
                            end_point = "D10";
                        }
                        else
                        {
                            end_point = Route.FormateStringIntoEquipmentName(end_point);
                        }
                        List<List<string>> greedy_path = new List<List<string>>();
                        int index = 0;
                        Route.GenEquipmentString(structure, greedy_path, index, start_point, end_point, direction);
                        string switch_string = Route.GenSwitchString(structure, greedy_path);
                        shunt_route.GeneratedSwitchString = switch_string;
                    }
                }
            }
            else
            {
                MessageBox.Show("输入数据不完整！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private bool CheckDataIntegrility()
        {
            bool flag = true;
            if (structure.Routes.Count == 0 ||
                structure.Sections.Count == 0||
                structure.Signals.Count==0||
                structure.Switches.Count == 0)
            {
                flag = false;
            }
            return flag;
        }
        #endregion
    }
}