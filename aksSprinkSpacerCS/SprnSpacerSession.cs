using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace aksSprinkSpacer
{
    public class AreaOp
    {
        private double _d_opAreaVal;
        public double DOpArea {
            get { return _d_opAreaVal; }
            set {
                _d_opAreaVal = value;
                _s_opAreaVal = value.ToString();
            }
        }

        private string _s_opAreaVal;
        public string SOpArea {
            get { return _s_opAreaVal; }
            set { _s_opAreaVal = value; }
        }
    }

    public class SprnkSpc
    {
        private double _d_sprnkspc;
        public double Dsprnkspc {
            get { return _d_sprnkspc; }
            set { _d_sprnkspc = value; }
        }

        private string _s_sprnkspc;
        public string Ssprnkspc {
            get { return _s_sprnkspc; }
            set { _s_sprnkspc = value; }
        }
    }

    public class SprnSpacerSession : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        // This is all about binding the sprinkler spacing combobox list and
        // the selected value (an object) in that list.
        private List<SprnkSpc> _sprnkspc = new List<SprnkSpc>(){
            new SprnkSpc(){ Dsprnkspc=15.0, Ssprnkspc="15 ft"}
            ,new  SprnkSpc(){ Dsprnkspc=14.0, Ssprnkspc="14 ft"}
            ,new  SprnkSpc(){ Dsprnkspc=10.0, Ssprnkspc="10 ft"}
            ,new  SprnkSpc(){ Dsprnkspc=8.0, Ssprnkspc="8 ft"}
            ,new  SprnkSpc(){ Dsprnkspc=16.0, Ssprnkspc="16 ft"}
            ,new  SprnkSpc(){ Dsprnkspc=18.0, Ssprnkspc="18 ft"}
            ,new  SprnkSpc(){ Dsprnkspc=20.0, Ssprnkspc="20 ft"}
        };

        public List<SprnkSpc> Sprnkspc {
            get { return _sprnkspc; }
            set {
                _sprnkspc = value;
                OnPropertyChanged("Sprnkspc");
            }
        }
        // The selected sprinkler spacing object
        private SprnkSpc _selsprnkspc = new SprnkSpc() { Dsprnkspc = 15.0, Ssprnkspc = "15 ft" }; // default
        public SprnkSpc Selsprnkspc {
            get { return _selsprnkspc; }
            set {
                _selsprnkspc = value;
                OnPropertyChanged("Selsprnkspc");
            }
        }


        // This is all about binding the area ops combobox list and
        // the selected value (an object) in that list.
        private List<AreaOp> _areaops = new List<AreaOp>(){
            new AreaOp(){ DOpArea=130.0, SOpArea="130 sf/sprnk"}
            ,new  AreaOp(){ DOpArea = 225.0, SOpArea="225 sf/sprnk"}
            ,new  AreaOp(){ DOpArea = 100.0, SOpArea="100 sf/sprnk"}
            ,new  AreaOp(){ DOpArea = 256.0, SOpArea="256 sf/sprnk"}
            ,new  AreaOp(){ DOpArea=324.0, SOpArea="324 sf/sprnk"}
            ,new  AreaOp(){ DOpArea=400.0, SOpArea="400 sf/sprnk"}
        };

        public List<AreaOp> Areaops {
            get { return _areaops; }
            set {
                _areaops = value;
                OnPropertyChanged("Areaops");
            }
        }

        // The selected area op
        private AreaOp _selareaop = new AreaOp() { DOpArea = 225.0, SOpArea = "225 sf/sprnk" }; // default
        public AreaOp SelAreaop {
            get { return _selareaop; }
            set {
                _selareaop = value;
                OnPropertyChanged("SelAreaop");
            }
        }

        private string str_numberOperating = "0";
        public string Str_NumberOperating { get { return str_numberOperating; } set { str_numberOperating = value; OnPropertyChanged("Str_NumberOperating"); } }

        private string str_totflow = "na";
        public string Str_Totflow { get { return str_totflow; } set { str_totflow = value; OnPropertyChanged("Str_Totflow"); } }

        private string str_eachBranchOperating = "0";
        public string Str_EachBranchOperating { get { return str_eachBranchOperating; } set { str_eachBranchOperating = value; OnPropertyChanged("Str_EachBranchOperating"); } }

        private string str_maxFrmWallL;
        public string Str_MaxFrmWallL { get { return str_maxFrmWallL; } set { str_maxFrmWallL = value; OnPropertyChanged("Str_MaxFrmWallL"); } }

        private string str_maxFrmWallW = "0.00";
        public string Str_MaxFrmWallW { get { return str_maxFrmWallW; } set { str_maxFrmWallW = value; OnPropertyChanged("Str_MaxFrmWallW"); } }

        private string str_branchQtyL = "0";
        public string Str_BranchQtyL { get { return str_branchQtyL; } set { str_branchQtyL = value; OnPropertyChanged("Str_BranchQtyL"); } }

        private string str_branchQtyW = "0";
        public string Str_BranchQtyW { get { return str_branchQtyW; } set { str_branchQtyW = value; OnPropertyChanged("Str_BranchQtyW"); } }

        private string str_sprinklerQty = "0";
        public string Str_SprinklerQty { get { return str_sprinklerQty; } set { str_sprinklerQty = value; OnPropertyChanged("Str_SprinklerQty"); } }

        private string str_btwSprnkDimL = "0.00";
        public string Str_BtwSprnkDimL { get { return str_btwSprnkDimL; } set { str_btwSprnkDimL = value; OnPropertyChanged("Str_BtwSprnkDimL"); } }

        private string str_btwSprnkDimW = "0.00";
        public string Str_BtwSprnkDimW { get { return str_btwSprnkDimW; } set { str_btwSprnkDimW = value; OnPropertyChanged("Str_BtwSprnkDimW"); } }

        private string str_roomLength = "10";
        public string Str_RoomLength {
            get { return str_roomLength; }
            set {
                str_roomLength = value;
                if (string.IsNullOrEmpty(value) || !IsNumeric(value))
                {
                    dbl_RoomLength = 0.0;
                }
                else
                {
                    dbl_RoomLength = Convert.ToDouble(value);
                }
                OnPropertyChanged("Str_RoomLength");
            }
        }
        private double dbl_RoomLength = 10.0;
        public double Dbl_RoomLength {
            get { return dbl_RoomLength; }
            set {
                dbl_RoomLength = value;
                str_roomLength = dbl_RoomLength.ToString("0.00");
                OnPropertyChanged("Dbl_RoomLength");
                OnPropertyChanged("Str_RoomLength");
            }
        }

        private string str_RoomLenFtPart = "10";
        public string Str_RoomLenFtPart {
            get { return str_RoomLenFtPart; }
            set {
                str_RoomLenFtPart = value;
                Double.TryParse(value, out dbl_RoomLenFtPart);
                OnPropertyChanged("Str_RoomLenFtPart");
                OnPropertyChanged("Dbl_RoomLenFtPart");
            }
        }

        private double dbl_RoomLenFtPart = 10.0;
        public double Dbl_RoomLenFtPart { get { return dbl_RoomLenFtPart; } set { dbl_RoomLenFtPart = value; OnPropertyChanged("Dbl_RoomLenFtPart"); } }

        private string str_RoomLenInPart = "0";
        public string Str_RoomLenInPart {
            get { return str_RoomLenInPart; }
            set {
                str_RoomLenInPart = value;
                Double.TryParse(value, out dbl_RoomLenInPart);
                OnPropertyChanged("Str_RoomLenInPart");
                OnPropertyChanged("Dbl_RoomLenInPart");
            }
        }
        private double dbl_RoomLenInPart = 0.0;
        public double Dbl_RoomLenInPart { get { return dbl_RoomLenInPart; } set { dbl_RoomLenInPart = value; OnPropertyChanged("Dbl_RoomLenInPart"); } }

        private string str_RoomWidFtPart = "12";
        public string Str_RoomWidFtPart {
            get { return str_RoomWidFtPart; }
            set {
                str_RoomWidFtPart = value;
                Double.TryParse(value, out dbl_RoomWidFtPart);
                OnPropertyChanged("Str_RoomWidFtPart");
                OnPropertyChanged("Dbl_RoomWidFtPart");
            }
        }
        private double dbl_RoomWidFtPart = 12.0;
        public double Dbl_RoomWidFtPart { get { return dbl_RoomWidFtPart; } set { dbl_RoomWidFtPart = value; OnPropertyChanged("Dbl_RoomWidFtPart"); } }

        private string str_RoomWidInPart = "0";
        public string Str_RoomWidInPart {
            get { return str_RoomWidInPart; }
            set {
                str_RoomWidInPart = value;
                Double.TryParse(value, out dbl_RoomWidInPart);
                OnPropertyChanged("Str_RoomWidInPart");
                OnPropertyChanged("Dbl_RoomWidInPart");
            }
        }
        private double dbl_RoomWidInPart = 0.0;
        public double Dbl_RoomWidInPart { get { return dbl_RoomWidInPart; } set { dbl_RoomWidInPart = value; OnPropertyChanged("Dbl_RoomWidInPart"); } }

        private string str_spnkK = "5.6";
        public string Str_SpnkK {
            get { return str_spnkK; }
            set {
                str_spnkK = value;
                dbl_spnkK = Convert.ToDouble(value);
                OnPropertyChanged("Str_SpnkK");
                OnPropertyChanged("Dbl_SpnkK");
            }
        }
        private double dbl_spnkK = 5.6;
        public double Dbl_SpnkK { get { return dbl_spnkK; } set { dbl_spnkK = value; OnPropertyChanged("Dbl_SpnkK"); } }


        private string str_dDensity = "0.10";
        public string Str_DDensity {
            get { return str_dDensity; }
            set {
                str_dDensity = value;
                Double.TryParse(value, out double DD);
                dbl_dDensity = DD;
                OnPropertyChanged("Str_DDensity");
                OnPropertyChanged("Dbl_DDensity");
            }
        }
        private double dbl_dDensity;
        public double Dbl_DDensity { get { return dbl_dDensity; } set { dbl_dDensity = value; OnPropertyChanged("Dbl_DDensity"); } }

        private double dbL_maxSprnkOpArea = 225;
        public double Dbl_MaxSprnkOpArea { get { return dbL_maxSprnkOpArea; } set { dbL_maxSprnkOpArea = value; OnPropertyChanged("Dbl_MaxSprnkOpArea"); } }

        private double dbl_maxSprnkSpacing = 15;
        public double Dbl_MaxSprnkSpacing { get { return dbl_maxSprnkSpacing; } set { dbl_maxSprnkSpacing = value; OnPropertyChanged("Dbl_MaxSprnkSpacing"); } }

        private string str_roomWid = "0.00";
        public string Str_RoomWid {
            get { return str_roomWid; }
            set {
                str_roomWid = value;
                if (string.IsNullOrEmpty(value) || !IsNumeric(value))
                {
                    dbl_RoomWidth = 0.0;
                }
                else
                {
                    dbl_RoomWidth = Convert.ToDouble(value);
                }
                OnPropertyChanged("Str_RoomWid");
            }
        }
        private double dbl_RoomWidth = 10.0;
        public double Dbl_RoomWidth {
            get { return dbl_RoomWidth; }
            set {
                dbl_RoomWidth = value;
                str_roomWid = dbl_RoomWidth.ToString("0.00");
                OnPropertyChanged("Dbl_RoomWidth");
                OnPropertyChanged("Str_RoomWid");
            }
        }

        private string str_calcOptLSpace;
        public string Str_calcOptLSpace {
            get { return str_calcOptLSpace; }
            set {
                str_calcOptLSpace = value;
                dbl_calcOptLSpace = Convert.ToDouble(value);
                dbl_oparea = dbl_calcOptLSpace * dbl_calcOptWSpace;
                OnPropertyChanged("Str_calcOptLSpace");
            }
        }

        private double dbl_calcOptLSpace;
        public double Dbl_CalcOptLSpace { get { return dbl_calcOptLSpace; } set { dbl_calcOptLSpace = value; OnPropertyChanged("Dbl_CalcOptLSpace"); } }

        private string str_calcOptWSpace;
        public string Str_calcOptWSpace {
            get { return str_calcOptWSpace; }
            set {
                str_calcOptWSpace = value;
                dbl_calcOptWSpace = Convert.ToDouble(value);
                dbl_oparea = dbl_calcOptLSpace * dbl_calcOptWSpace;
                OnPropertyChanged("Str_calcOptWSpace");
            }
        }

        private double dbl_calcOptWSpace;
        public double Dbl_CalcOptWSpace { get { return dbl_calcOptWSpace; } set { dbl_calcOptWSpace = value; OnPropertyChanged("Dbl_CalcOptWSpace"); } }

        private double dbl_minimumpressure;
        public double Dbl_Minimumpressure {
            get { return dbl_minimumpressure; }
            set {
                dbl_minimumpressure = value;
                if (value > 0)
                {
                    str_minpressure = Math.Max(7, value).ToString("0.00") + " psi";
                }
                else
                {
                    str_minpressure = "na";
                }
                OnPropertyChanged("Dbl_Minimumpressure");
                OnPropertyChanged("Str_Minpressure");
            }
        }

        private string str_minpressure = "na";
        public string Str_Minpressure { get { return str_minpressure; } set { str_minpressure = value; OnPropertyChanged("Str_Minpressure"); } }

        private double dbl_designArea = 1500;
        public double Dbl_DesignArea { get { return dbl_designArea; } set { dbl_designArea = value; OnPropertyChanged("Dbl_DesignArea"); } }

        private string str_designArea = "1500";
        public string Str_DesignArea { get { return str_designArea; } set { str_designArea = value; OnPropertyChanged("Str_DesignArea"); } }

        private int sprnkQtyFlowing;
        public int SprnkQtyFlowing {
            get { return sprnkQtyFlowing; }
            set {
                sprnkQtyFlowing = value;
                str_sprnkQtyFlowing = sprnkQtyFlowing.ToString();
                OnPropertyChanged("SprnkQtyFlowing");
                OnPropertyChanged("Str_SprnkQtyFlowing");
            }
        }
        private string str_sprnkQtyFlowing;
        public string Str_SprnkQtyFlowing { get { return str_sprnkQtyFlowing; } set { str_sprnkQtyFlowing = value; OnPropertyChanged("Str_SprnkQtyFlowing"); } }

        private int sprnkQtyEachBranchLine;
        public int SprnkQtyEachBranchLine {
            get { return sprnkQtyFlowing; }
            set {
                sprnkQtyEachBranchLine = value;
                str_sprnkQtyEachBranchLine = sprnkQtyEachBranchLine.ToString();
                OnPropertyChanged("SprnkQtyEachBranchLine");
                OnPropertyChanged("Str_SprnkQtyEachBranchLine");
            }
        }
        private string str_sprnkQtyEachBranchLine;
        public string Str_SprnkQtyEachBranchLine { get { return str_sprnkQtyEachBranchLine; } set { str_sprnkQtyEachBranchLine = value; OnPropertyChanged("Str_SprnkQtyEachBranchLine"); } }

        private string str_whatdominates = "na";
        public string Str_Whatdominates { get { return str_whatdominates; } set { str_whatdominates = value; OnPropertyChanged("Str_Whatdominates"); } }

        private string str_oparea = "na";
        public string Str_Oparea { get { return str_oparea; } set { str_oparea = value; OnPropertyChanged("Str_Oparea"); } }

        private double dbl_oparea = 0;
        public double Dbl_Oparea {
            get { return dbl_oparea; }
            set {
                dbl_oparea = value;
                if (value > 0)
                {
                    str_oparea = Math.Max(7, value).ToString("0.00") + " sf/sprnk";
                }
                else
                {
                    str_oparea = "na";
                }
                OnPropertyChanged("Dbl_Oparea");
                OnPropertyChanged("Str_Oparea");
            }
        }

        public static Boolean IsNumeric(String input)
        {
            Boolean result = Double.TryParse(input, out double temp);
            return result;
        }
    }
}
