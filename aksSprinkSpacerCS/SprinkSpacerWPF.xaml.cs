using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace aksSprinkSpacer {
    /// <summary>
    /// Interaction logic for SprinkSpacerWPF.xaml
    /// </summary>
    public partial class SprinkSpacerWPF : Window {
        SprnSpacerSession ss = new SprnSpacerSession();
        object[] CalcsResult = new object[11];
        int ReqQty;
        bool BeSilent = false;
        bool DesignErrorOpArea = false;
        bool DesignErrorSpacing = false;
        object[] resNada = { "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A", "N/A" };
        DispatcherTimer timeOut = new DispatcherTimer();

        public SprinkSpacerWPF() {
            BeSilent = true;
            InitializeComponent();
            BeSilent = false;
        }

        public void GetInits() {
            BeSilent = true;
            Top = Properties.Settings.Default.Loc_Top;
            Left = Properties.Settings.Default.Loc_Left;
            ss.Dbl_RoomLength = Properties.Settings.Default.RoomLen;
            ss.Dbl_RoomWidth = Properties.Settings.Default.RoomWid;

            AreaOp areaOp = ss.Areaops.Find(opa => opa.SOpArea.Equals(Properties.Settings.Default.OpArea));
            if (areaOp != null)
            {
                ss.SelAreaop = areaOp;
                ss.Dbl_MaxSprnkOpArea = areaOp.DOpArea;
            }
            else
            {
                ss.SelAreaop = new AreaOp() { DOpArea = 225.0, SOpArea = "225 sf/sprnk" };
                ss.Dbl_MaxSprnkOpArea = ss.SelAreaop.DOpArea;
            }

            SprnkSpc sprnkSpc = ss.Sprnkspc.Find(spc => spc.Ssprnkspc.Equals(Properties.Settings.Default.MaxSpace));
            if (sprnkSpc != null) {
                ss.Selsprnkspc = sprnkSpc;
                ss.Dbl_MaxSprnkSpacing = sprnkSpc.Dsprnkspc;
            } else {
                ss.Selsprnkspc = new SprnkSpc() { Dsprnkspc = 15.0, Ssprnkspc = "15 ft" }; // default
                ss.Dbl_MaxSprnkSpacing = ss.Selsprnkspc.Dsprnkspc;
            };

            ss.Str_SpnkK = Properties.Settings.Default.SprnkK;
            ss.Str_DDensity = Properties.Settings.Default.DesDensity;
            ss.Str_DesignArea = Properties.Settings.Default.DesArea;
            BeSilent = false;
        }

        public void SaveState() {
            Properties.Settings.Default.Loc_Top = Top;
            Properties.Settings.Default.Loc_Left = Left;
            Properties.Settings.Default.RoomLen = ss.Dbl_RoomLength;
            Properties.Settings.Default.RoomWid = ss.Dbl_RoomWidth;
            Properties.Settings.Default.OpArea = ss.SelAreaop.SOpArea;
            Properties.Settings.Default.MaxSpace = ss.Selsprnkspc.Ssprnkspc;
            Properties.Settings.Default.SprnkK = ss.Str_SpnkK;
            Properties.Settings.Default.DesDensity = ss.Str_DDensity;
            Properties.Settings.Default.DesArea = ss.Str_DesignArea;
            Properties.Settings.Default.Save();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            DataContext = ss;
            GetInits();
            RunAllCalcs();
            timeOut.Tick += new EventHandler(TimeOut_Tick);
            timeOut.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            SaveState();
        }

        // A Frankenstein!!! Should be refactored.
        private void OpCalcs() {
            try {
                if (ss.Dbl_DesignArea == 0) {
                    ss.Str_Totflow = "na";
                    return;
                }

                double OptLS = ss.Dbl_CalcOptLSpace;
                double OptWS = ss.Dbl_CalcOptWSpace;

                if (OptLS > 0 & OptWS > 0) {
                    ss.SprnkQtyFlowing = (int)Math.Ceiling(ss.Dbl_DesignArea / (OptLS * OptWS));
                    ss.SprnkQtyEachBranchLine = (int)Math.Ceiling(1.2 * (Math.Sqrt(ss.Dbl_DesignArea) / OptLS));

                    double TDGPM = Math.Ceiling(ss.Dbl_DesignArea / (OptLS * OptWS))
                        * ss.Dbl_SpnkK * Math.Sqrt(ss.Dbl_Minimumpressure);
                    ss.Str_Totflow = Convert.ToString(Math.Round(TDGPM, 1)) + " gpm";
                } else {
                    ss.Str_Totflow = "na";
                }

            } catch (Exception) {
            }
        }

        private void CalcSprnkForm() {
            if (BeSilent) { return; }
            if (ss.Dbl_RoomLength > 0 & ss.Dbl_RoomWidth > 0) {

                ss.Dbl_MaxSprnkOpArea = ss.SelAreaop.DOpArea;
                ss.Dbl_MaxSprnkSpacing = ss.Selsprnkspc.Dsprnkspc;

                CalcsResult = SpLayout(ss.Dbl_RoomLength, ss.Dbl_RoomWidth, ss.Dbl_MaxSprnkOpArea, ss.Dbl_MaxSprnkSpacing);
                if (!CalcsResult.Equals(resNada)) {
                    ReqQty = (int)Math.Min((double)CalcsResult[0] * (double)CalcsResult[1], (double)CalcsResult[4] * (double)CalcsResult[5]);
                    ss.Str_SprinklerQty = ReqQty.ToString();
                    double oplsp = (double)CalcsResult[2];
                    ss.Dbl_CalcOptLSpace = Convert.ToDouble(oplsp.ToString("0.00"));
                    double opksp = (double)CalcsResult[3];
                    ss.Dbl_CalcOptWSpace = Convert.ToDouble(opksp.ToString("0.00"));
                    ss.Str_calcOptLSpace = ss.Dbl_CalcOptLSpace.ToString();
                    ss.Str_calcOptWSpace = ss.Dbl_CalcOptWSpace.ToString();
                    ss.Str_Whatdominates = CalcsResult[9] as string;
                }
            }
        }

        private void CalcAreaPerSprnk() {
            double T = 0;
            double L = ss.Dbl_CalcOptLSpace;
            double W = ss.Dbl_CalcOptWSpace;
            double maxSpc = ss.Selsprnkspc.Dsprnkspc;
            double opArea = ss.SelAreaop.DOpArea;

            T = L * W;
            ss.Dbl_Oparea = T;

            if (L > maxSpc || W > maxSpc) {
                DesignErrorSpacing = true;
            } else {
                DesignErrorSpacing = false;
            }

            if (T > opArea) {
                DesignErrorOpArea = true;
            } else {
                DesignErrorOpArea = false;
            }
            AnyDesignErrors();
        }

        private bool AnyDesignErrors() {
            if (DesignErrorOpArea) {
                AppBody.Background = ColorExt.ToBrush(System.Drawing.Color.LightPink);
                return true;
            }
            if (DesignErrorSpacing) {
                AppBody.Background = ColorExt.ToBrush(System.Drawing.Color.LightPink);
                return true;
            }
            AppBody.Background = ColorExt.ToBrush(System.Drawing.Color.AliceBlue);
            return false;
        }

        private void CalcPressure() {
            double K = ss.Dbl_SpnkK;
            double Density = ss.Dbl_DDensity;
            double L = ss.Dbl_CalcOptLSpace;
            double W = ss.Dbl_CalcOptWSpace;

            if (K == 0 || Density == 0) {
                ss.Dbl_Minimumpressure = 0;
                return;
            }

            double OpArea = L * W;
            double RP = Math.Pow((Density * OpArea / K), 2);
            ss.Dbl_Minimumpressure = RP;
        }

        // Function SpLayout returns the most efficient sprinkler spacing in a L by W
        // area given the max area per sprinkler and the max spacing.  The spacing is balanced
        // out over the L and W dimensions. (eg. no attempt to meet a ceiling grid spacing)
        //
        // Splayout returns an Array.  Use control+shift+return over the number of array cells
        // to enter in the formula
        //
        // AKS 12/31/01
        //
        public object[] SpLayout(double RoomLength, double RoomWidth, double MaxSprnkOpArea, double MaxSprnkSpacing) {
            double Lnuml = 0;
            //spk num in L with L dominating
            double Lspl = 0;
            //spk spacing in L with L dominating
            double Lnumw = 0;
            //spk num in W with L dominating
            double Lspw = 0;
            //spk spacing in W with L dominating
            double Larea = 0;
            // area per spk with L dominating
            double Wnuml = 0;
            //spk num in L with W dominating
            double Wspl = 0;
            //spk spacing in L with W dominating
            double Wnumw = 0;
            //spk num in W with W dominating
            double Wspw = 0;
            //spk spacing in W with W dominating
            double Warea = 0;
            // area per spk with W dominating
            double AreaDif = 0;
            // difference between op areas
            int SpkDif = 0;
            //diiference in total number of sprnk
            int Evenl = 0;
            int EvenW = 0;
            double MSPL = 0;
            double MSPW = 0;

            if (RoomLength == 0 | RoomWidth == 0) {
                return resNada;
            }
            if (MaxSprnkSpacing == 0 | MaxSprnkSpacing == 0) {
                return resNada;
            }

            MSPL = MaxSprnkSpacing;
            MSPW = MaxSprnkSpacing;
            Evenl = 0;
            EvenW = 0;
            if (Evenl > 0)
                MSPL = SpkEven(MaxSprnkSpacing);
            if (EvenW > 0)
                MSPW = SpkEven(MaxSprnkSpacing);

            try {
                // Calculate with L dominating
                Lnuml = Math.Ceiling(RoomLength / MSPL);
                Lspl = RoomLength / Lnuml;
                if (Evenl > 0)
                    Lspl = SpkEven(RoomLength / SpkEven(Lnuml));
                Lnumw = Math.Ceiling(RoomWidth / Math.Min(MSPW, (MaxSprnkOpArea / Lspl)));
                Lspw = Math.Max(6, RoomWidth / Lnumw);
                if (EvenW > 0)
                    Lspw = SpkEven(RoomWidth / SpkEven(Lnumw));
                Larea = Lspl * Lspw;
                // Calculate with W dominating
                Wnuml = Math.Ceiling(RoomWidth / MSPW);
                Wspl = Math.Max(6, RoomWidth / Wnuml);
                if (Evenl > 0)
                    Wspl = SpkEven(RoomWidth / SpkEven(Wnuml));
                Wnumw = Math.Ceiling(RoomLength / Math.Min(MSPL, (MaxSprnkOpArea / Wspl)));
                Wspw = Math.Max(6, RoomLength / Wnumw);
                if (EvenW > 0)
                    Wspw = SpkEven(RoomLength / SpkEven(Wnumw));
                Warea = Wspl * Wspw;
                // Comparisons
                AreaDif = Larea - Warea;
                SpkDif = (int)((Lnuml * Lnumw) - (Wnuml * Wnumw));

                object[] _spLayout = new object[11];
                // Return based on AreaDif
                if (AreaDif > 0) {
                    _spLayout = new object[] { Lnuml, Lnumw, Lspl, Lspw, Wnuml, Wnumw, Wspl, Wspw, Larea, "L", AreaDif, SpkDif };
                }
                if (AreaDif < 0) {
                    _spLayout = new object[] { Lnuml, Lnumw, Lspl, Lspw, Wnuml, Wnumw, Wspl, Wspw, Larea, "L", AreaDif, SpkDif };
                }

                if (AreaDif == 0) {
                    _spLayout = new object[] { Lnuml, Lnumw, Lspl, Lspw, Wnumw, Wnuml, Wspw, Wspl, Larea, "L,W", AreaDif, SpkDif };
                }
                return _spLayout;
            } catch (Exception) {
                return resNada;
            }
        }

        public int SpkEven(double Item) {
            int functionReturnValue = 0;
            functionReturnValue = (int)Math.Round(Item - 1);
            if ((functionReturnValue / 2) > Convert.ToInt32(functionReturnValue / 2)) {
                functionReturnValue = functionReturnValue + 1;
            }
            return functionReturnValue;
        }

        private void RunAllCalcs() {
            if (BeSilent) { return; }
            if (ss.Dbl_RoomLength > 0 & ss.Dbl_RoomWidth > 0) {
                CalcSprnkForm();
                CalcAreaPerSprnk();
                CalcPressure();
                OpCalcs();
                CalcMax();
            }
        }

        public static Boolean IsNumeric(String input) {
            Boolean result = Double.TryParse(input, out double temp);
            return result;
        }

        private void TextBoxRoomWid_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                AnyDesignErrors();
                RunAllCalcs();
            } catch (InvalidCastException) {
                AppBody.Background = ColorExt.ToBrush(System.Drawing.Color.LightPink);
            }
        }

        private void TextBoxSpnkK_TextChanged(object sender, TextChangedEventArgs e) {
            RunAllCalcs();
        }

        private void TextBoxDDensity_TextChanged(object sender, TextChangedEventArgs e) {
            RunAllCalcs();
        }

        private void TextBoxDesignArea_TextChanged(object sender, TextChangedEventArgs e) {
            RunAllCalcs();
        }

        private void TextBoxCalcOptLSpace_TextChanged(object sender, TextChangedEventArgs e) {
            CalcAreaPerSprnk();
            CalcPressure();
            OpCalcs();
            CalcMax();
        }

        private void CalcMax() {
            try {
                double LSpacing = ss.Dbl_CalcOptLSpace;
                double WSpacing = ss.Dbl_CalcOptWSpace;

                ss.Str_MaxFrmWallL = (LSpacing / 2).ToString("0.00");
                ss.Str_MaxFrmWallW = (WSpacing / 2).ToString("0.00");

                int QtyL = Convert.ToInt32(Math.Ceiling(Math.Round(ss.Dbl_RoomLength / LSpacing, 2)));
                int QtyW = Convert.ToInt32(Math.Ceiling(Math.Round(ss.Dbl_RoomWidth / WSpacing, 2)));

                ss.Str_BranchQtyL = QtyL.ToString();
                ss.Str_BranchQtyW = QtyW.ToString();
                ss.Str_SprinklerQty = (QtyL * QtyW).ToString();

                double LSP = (ss.Dbl_RoomLength - LSpacing) / (QtyL - 1);
                double WSP = (ss.Dbl_RoomWidth - WSpacing) / (QtyW - 1);

                ss.Str_BtwSprnkDimL = LSP.ToString("0.00");
                ss.Str_BtwSprnkDimW = WSP.ToString("0.00");

                AnyDesignErrors();
            } catch (Exception) {
                AppBody.Background = ColorExt.ToBrush(System.Drawing.Color.LightPink);
            }
        }

        private void TextBoxCalcOptWSpace_TextChanged(object sender, TextChangedEventArgs e) {
            CalcAreaPerSprnk();
            CalcPressure();
            OpCalcs();
            CalcMax();
        }

        public double ConvertToFeetDecimal(double _thisFoot, double _thisInches) {
            return (_thisFoot + _thisInches / 12);
        }

        private void RoomLengthFromParts() {
            ss.Dbl_RoomLength = ConvertToFeetDecimal(ss.Dbl_RoomLenFtPart, ss.Dbl_RoomLenInPart);
        }

        private void RoomWidthFromParts() {
            ss.Dbl_RoomWidth = ConvertToFeetDecimal(ss.Dbl_RoomWidFtPart, ss.Dbl_RoomWidInPart);
        }

        private void TextBoxRoomLen_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                AnyDesignErrors();
                RunAllCalcs();
            } catch (InvalidCastException) {
                AppBody.Background = ColorExt.ToBrush(System.Drawing.Color.LightPink);
            }
        }

        private void TextBoxRoomLenFt_TextChanged(object sender, TextChangedEventArgs e) {
            DoRoomLengthTextPartsChange();
            RunAllCalcs();
        }

        private void DoRoomLengthTextPartsChange() {
            if (!BeSilent) {
                RoomLengthFromParts();
            }
        }

        private void DoRoomWidthTextPartsChange() {
            if (!BeSilent) {
                RoomWidthFromParts();
            }
        }

        private void TextBoxRoomLenIn_TextChanged(object sender, TextChangedEventArgs e) {
            DoRoomLengthTextPartsChange();
            RunAllCalcs();
        }

        private void RoomDimensionsToParts() {
            RoomLengthToParts();
            RoomWidthToParts();
        }

        private void RoomLengthToParts() {
            bool silentState = BeSilent;
            BeSilent = true;
            double thisFoot = Math.Truncate(ss.Dbl_RoomLength);
            double thisInches = 12 * (ss.Dbl_RoomLength % 1);
            ss.Str_RoomLenFtPart = thisFoot.ToString();
            ss.Str_RoomLenInPart = thisInches.ToString("0.00");
            BeSilent = silentState;
        }

        private void RoomWidthToParts() {
            bool silentState = BeSilent;
            BeSilent = true;
            double thisFoot = Math.Truncate(ss.Dbl_RoomWidth);
            double thisInches = 12 * (ss.Dbl_RoomWidth % 1);
            ss.Str_RoomWidFtPart = thisFoot.ToString();
            ss.Str_RoomWidInPart = thisInches.ToString("0.00");
            BeSilent = silentState;
        }

        private void TextBoxRoomWidFT_TextChanged(object sender, TextChangedEventArgs e) {
            DoRoomWidthTextPartsChange();
            RunAllCalcs();
        }

        private void TextBoxRoomWidIN_TextChanged(object sender, TextChangedEventArgs e) {
            DoRoomWidthTextPartsChange();
            RunAllCalcs();
        }

        private void TimeOut_Tick(object sender, EventArgs e) {
            timeOut.Stop();
            RoomDimensionsToParts();
        }

        private void TextBoxRoomLen_KeyUp(object sender, KeyEventArgs e) {
            StartEntryTimer();
        }

        private void TextBoxRoomWid_KeyUp(object sender, KeyEventArgs e) {
            StartEntryTimer();
        }

        private void StartEntryTimer() {
            timeOut.Stop();
            timeOut.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timeOut.Start();
        }

        private void TextBoxRoomLen_MouseClick(object sender, MouseEventArgs e) {
            RunAllCalcs();
        }

        public void DragWindow(object sender, MouseButtonEventArgs args) {
            // Watch out. Fatal error if not primary button!
            if (args.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }

        private void ComboBoxMaxSprinkOpArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ss.Dbl_MaxSprnkOpArea = ss.SelAreaop.DOpArea;
            RunAllCalcs();
        }

        private void ComboMaxSpaceChanged(object sender, SelectionChangedEventArgs e)
        {
            ss.Dbl_MaxSprnkSpacing = ss.Selsprnkspc.Dsprnkspc;
            RunAllCalcs();
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            RunAllCalcs();
        }

        private void TextBoxRoomLenFt_LostFocus(object sender, RoutedEventArgs e) {
            RoomDimensionsToParts();
        }

        private void TextBoxRoomLenIn_LostFocus(object sender, RoutedEventArgs e) {
            RoomDimensionsToParts();
        }

        private void TextBoxRoomWidFT_LostFocus(object sender, RoutedEventArgs e) {
            RoomDimensionsToParts();
        }

        private void TextBoxRoomWidIN_LostFocus(object sender, RoutedEventArgs e) {
            RoomDimensionsToParts();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape) {
                Close();
            }
        }
        
    }

    /// <summary>
    /// Used to convert system drawing colors to WPF brush
    /// </summary>
    public static class ColorExt {
        public static System.Windows.Media.Brush ToBrush(System.Drawing.Color color) {
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
    
}
