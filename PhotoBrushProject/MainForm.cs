using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Painting.Shapes;
using Crom.Controls.Docking;
using System.Drawing.Imaging;
using System.Collections;
using AForge.Imaging;

namespace PhotoBrushProject
{
    public enum FormStyle
    {
        StartPage, Histogram, Details, ToolBox, ImageHandleForm, PaintHandleForm, PsdHandlerForm
    }

    public partial class MainForm : Form, IDocumentsHost
    {
        private static string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "app.config");
        private static string dockManagerConfigFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockManager.config");

        private int unnamedNumber = 0;
        private Configuration config = new Configuration();

        DockStateSerializer DSSerializer = null;

        public StartPageForm SPF = null;
        public HistogramForm HF = null;
        public DetailsForm DF = null;

        public Dictionary<FormStyle, int> EnableActivatedEventDictionary = null;
        public List<ImageHandlerForm> IHFList = null;

        public bool EnableActivePHF = true;
        public PaintHandlerForm ActivePHF = null;

        public bool EnableActiveIHF = true;
        public ImageHandlerForm ActiveIHF = null;

        public DockableFormInfo DFI01, DFI02, DFI03;//, DFI04;

        public MainForm()
        {
            MottoSplash MySplash = new MottoSplash();
            this.Visible = false;
            InitializeComponent();
            MySplash.ShowDialog();
            this.Visible = true;

            EnableActivatedEventDictionary = new Dictionary<FormStyle, int>();
            IHFList = new List<ImageHandlerForm>();
            DSSerializer = new DockStateSerializer(DockContainer01Panel);
            Initialize();
            //load configuration
            if (config.Load(configFile))
            {
                this.Location = config.mainWindowLocation;
                ////this.Size = config.mainWindowSize;
            }
        }

        void Initialize()
        {
            // Start Page Form
            SPF = (StartPageForm)FormCreator(FormStyle.StartPage, 0, 0, 725, 390, this.BackColor, "Start Page");
            DFI01 = DockContainer01Panel.Add(SPF, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d001"));
            DFI01.ShowContextMenuButton = false; //DFI01.ShowCloseButton = false;
            DFI01.ShowFormAutoPanel();
            DockContainer01Panel.DockForm(DFI01, DockStyle.Fill, zDockMode.Inner);

            //Details Form
            DF = (DetailsForm)FormCreator(FormStyle.Details, 0, 0, 300,300, this.BackColor, "Details View");
            DFI03 = DockContainer01Panel.Add(DF, zAllowedDock.All, new Guid("096b52a7-5f4b-44ee-ab77-9830ec717002"));
            DFI03.ShowContextMenuButton = true;
            DockContainer01Panel.DockForm(DFI03, DockStyle.Right, zDockMode.Inner);

            //Histogram Form
            HF = (HistogramForm)FormCreator(FormStyle.Histogram, 0, 0, 300,300, Color.White, "Histogram View");
            DFI02 = DockContainer01Panel.Add(HF, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d001"));
            DFI02.ShowContextMenuButton = true;
            DockContainer01Panel.DockForm(DFI02, DFI03, DockStyle.Bottom, zDockMode.Outer);

            //Paint Handle Form
            //PHF = (PaintHandlerForm)FormCreator(FormStyle.PaintHandleForm, 0, 0, 725, 390, Color.FromArgb(45, 45, 48), "Untiteled");
            //DFI04 = DockContainer01Panel.Add(PHF, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d001"));
            //DFI04.ShowContextMenuButton = false;
            //DockContainer01Panel.DockForm(DFI04, DFI01, DockStyle.Fill, zDockMode.Inner);
        }

        public void Initialize(IFormBuilder InstanceForm, FormStyle FS)
        {
            switch (FS)
            {
                case FormStyle.StartPage:
                    break;
                case FormStyle.Histogram:
                    break;
                case FormStyle.Details:
                    break;
                case FormStyle.ToolBox:
                    break;
                case FormStyle.PaintHandleForm:
                    //InstanceForm = (PaintHandlerForm)FormCreator(FormStyle.PaintHandleForm, 0, 0, 725, 390, Color.FromArgb(45, 45, 48), ((PaintHandlerForm)InstanceForm).Text);
                    DockableFormInfo DFI04 = DockContainer01Panel.Add((PaintHandlerForm)InstanceForm, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d001"));
                    DFI04.ShowContextMenuButton = false;
                    if (DockContainer01Panel.GetFormInfo(StartPageForm.Instance) != null)
                        DockContainer01Panel.DockForm(DFI04, DFI01, DockStyle.Fill, zDockMode.Inner);
                    else
                        DockContainer01Panel.DockForm(DFI04, DockStyle.Fill, zDockMode.Inner);
                    if (!EnableActivatedEventDictionary.ContainsKey(FormStyle.PaintHandleForm))
                        EnableActivatedEventDictionary.Add(FormStyle.PaintHandleForm, 0);
                    EnableActivatedEventDictionary[FormStyle.PaintHandleForm]++;
                    break;
                case FormStyle.ImageHandleForm:
                    //InstanceForm = (ImageHandlerForm)FormCreator(FS, 0, 0, 725, 390, Color.FromArgb(45, 45, 48), ((ImageHandlerForm)InstanceForm).Text);
                    DockableFormInfo DFI05 = DockContainer01Panel.Add((ImageHandlerForm)InstanceForm, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d001"));
                    DFI05.ShowContextMenuButton = false;
                    if (DockContainer01Panel.GetFormInfo(StartPageForm.Instance) != null)
                        DockContainer01Panel.DockForm(DFI05, DFI01, DockStyle.Fill, zDockMode.Inner);
                    else
                        DockContainer01Panel.DockForm(DFI05, DockStyle.Fill, zDockMode.Inner);
                    if (!EnableActivatedEventDictionary.ContainsKey(FormStyle.ImageHandleForm))
                        EnableActivatedEventDictionary.Add(FormStyle.ImageHandleForm, 0);
                    EnableActivatedEventDictionary[FormStyle.ImageHandleForm]++;
                    break;
                case FormStyle.PsdHandlerForm:
                    DockableFormInfo DFI06 = DockContainer01Panel.Add((PsdHandlerForm)InstanceForm, zAllowedDock.All, new Guid("a6402b80-2ebd-4fd3-8930-024a6201d001"));
                    DFI06.ShowContextMenuButton = false;
                    if(DockContainer01Panel.GetFormInfo(StartPageForm.Instance) != null)
                        DockContainer01Panel.DockForm(DFI06, DFI01, DockStyle.Fill, zDockMode.Inner);
                    else
                        DockContainer01Panel.DockForm(DFI06, DockStyle.Fill, zDockMode.Inner);
                    if (!EnableActivatedEventDictionary.ContainsKey(FormStyle.PsdHandlerForm))
                        EnableActivatedEventDictionary.Add(FormStyle.PsdHandlerForm, 0);
                    EnableActivatedEventDictionary[FormStyle.PsdHandlerForm]++;
                    break;
                default:
                    break;
            }
        }

        public IFormBuilder FormCreator(FormStyle FS, int Left, int Top, int Width, int Height, Color BackColor, string Title)
        {
            IFormBuilder IFB = null;
            switch (FS)
            {
                case FormStyle.StartPage:
                    IFB = StartPageForm.Instance;
                    ((StartPageForm)IFB).SetMainForm(this);
                    if (!EnableActivatedEventDictionary.ContainsKey(FormStyle.StartPage))
                        EnableActivatedEventDictionary.Add(FormStyle.StartPage, 0);
                    EnableActivatedEventDictionary[FormStyle.StartPage]++;
                    break;
                case FormStyle.Histogram:
                    IFB = HistogramForm.Instance;
                    if (!EnableActivatedEventDictionary.ContainsKey(FormStyle.Histogram))
                        EnableActivatedEventDictionary.Add(FormStyle.Histogram, 0);
                    EnableActivatedEventDictionary[FormStyle.Histogram]++;
                    break;
                case FormStyle.Details:
                    IFB = DetailsForm.Instance;
                    if (!EnableActivatedEventDictionary.ContainsKey(FormStyle.Details))
                        EnableActivatedEventDictionary.Add(FormStyle.Details, 0);
                    EnableActivatedEventDictionary[FormStyle.Details]++;
                    break;
                case FormStyle.ToolBox:
                    IFB = ToolBoxForm.Instance;
                    break;
                case FormStyle.PaintHandleForm:
                    IFB = new PaintHandlerForm(this);
                    if (!EnableActivatedEventDictionary.ContainsKey(FormStyle.PaintHandleForm))
                        EnableActivatedEventDictionary.Add(FormStyle.PaintHandleForm, 0);
                    EnableActivatedEventDictionary[FormStyle.PaintHandleForm]++;
                    break;
                case FormStyle.ImageHandleForm:
                    //IFB = new ImageHandlerForm(this);
                    //if (!EnableActivatedEventDictionary.ContainsKey(FormStyle.ImageHandleForm))
                    //    EnableActivatedEventDictionary.Add(FormStyle.ImageHandleForm, 0);
                    //EnableActivatedEventDictionary[FormStyle.ImageHandleForm]++;
                    break;
                default:
                    break;
            }
            IFB.Bounds = new Rectangle(Left, Top, Width, Height);
            IFB.FormBorderStyled = FormBorderStyle.SizableToolWindow;
            IFB.BackColor = BackColor;
            IFB.Text = Title;
            IFB.TopLevel = true;
            return IFB;
        }

        public void ChildFormEnableTools(FormStyle FS)
        {
            switch (FS)
            {
                case FormStyle.PaintHandleForm:
                    // Tool Strip
                    ToolStripButton04Save.Enabled = ToolStripButton05SaveAs.Enabled = true;
                    ToolStripButton07Copy.Enabled = ToolStripButton08Past.Enabled = true;
                    ToolStripButton06Cut.Enabled = ToolStripButton09Undo.Enabled = ToolStripButton10Redo.Enabled = false;
                    ToolStripButton11AntiAlias.Enabled = ToolStripButton12ShowClipping.Enabled = ToolStripSplitButton13Shapes.Enabled =
                        ToolStripButton14ConvertToCurve.Enabled = ToolStripButton15ConvertToLine.Enabled = ToolStripButton16DeleteNode.Enabled =
                        ToolStripButton17SelectionTool.Enabled = ToolStripButton18NodeTool.Enabled =
                        ToolStripButton19BringToFront.Enabled = ToolStripButton20SendToBack.Enabled = true;
                    //Menu Strip
                    printToolStripMenuItem.Enabled = printPreviewToolStripMenuItem.Enabled = printSetupToolStripMenuItem.Enabled = false;
                    cutToolStripMenuItem.Enabled = undoToolStripMenuItem.Enabled = redoToolStripMenuItem.Enabled = false;
                    copyToolStripMenuItem.Enabled = pasteToolStripMenuItem.Enabled = true;
                    imageToolStripMenuItem.Visible = filterToolStripMenuItem.Visible = false;
                    openInnewDocumentOnChangeToolStripMenuItem.Visible = rememberOnChangeToolStripMenuItem.Visible = false;
                    //Status Strip
                    PositionToolStripStatusLabel.Visible = true;zoomPanel.Visible = sizePanel.Visible = false;
                    break;
                case FormStyle.ImageHandleForm:
                    // Tool Strip
                    ToolStripButton04Save.Enabled = ToolStripButton05SaveAs.Enabled = ToolStripButton07Copy.Enabled = ToolStripButton08Past.Enabled = true;
                    ToolStripButton06Cut.Enabled = false;
                    ToolStripButton09Undo.Enabled = ToolStripButton10Redo.Enabled = false;
                    ToolStripButton11AntiAlias.Enabled = ToolStripButton12ShowClipping.Enabled = ToolStripSplitButton13Shapes.Enabled =
                        ToolStripButton14ConvertToCurve.Enabled = ToolStripButton15ConvertToLine.Enabled = ToolStripButton16DeleteNode.Enabled =
                        ToolStripButton17SelectionTool.Enabled = ToolStripButton18NodeTool.Enabled =
                        ToolStripButton19BringToFront.Enabled = ToolStripButton20SendToBack.Enabled = false;
                    //Menu Strip
                    imageToolStripMenuItem.Visible = filterToolStripMenuItem.Visible = true;
                    openInnewDocumentOnChangeToolStripMenuItem.Visible = rememberOnChangeToolStripMenuItem.Visible = true;
                    //Status Strip
                    PositionToolStripStatusLabel.Visible = zoomPanel.Visible = sizePanel.Visible = true;
                    break;
                case FormStyle.PsdHandlerForm:
                case FormStyle.StartPage:
                    // Tool Strip
                    ToolStripButton04Save.Enabled = ToolStripButton05SaveAs.Enabled = false;
                    ToolStripButton06Cut.Enabled = ToolStripButton07Copy.Enabled = ToolStripButton08Past.Enabled = false;
                    ToolStripButton09Undo.Enabled = ToolStripButton10Redo.Enabled = false;
                    ToolStripButton11AntiAlias.Enabled = ToolStripButton12ShowClipping.Enabled = ToolStripSplitButton13Shapes.Enabled =
                        ToolStripButton14ConvertToCurve.Enabled = ToolStripButton15ConvertToLine.Enabled = ToolStripButton16DeleteNode.Enabled =
                        ToolStripButton17SelectionTool.Enabled = ToolStripButton18NodeTool.Enabled =
                        ToolStripButton19BringToFront.Enabled = ToolStripButton20SendToBack.Enabled = false;
                    //Menu Strip
                    printToolStripMenuItem.Enabled = printPreviewToolStripMenuItem.Enabled = printSetupToolStripMenuItem.Enabled = false;
                    undoToolStripMenuItem.Enabled = redoToolStripMenuItem.Enabled = false;
                    cutToolStripMenuItem.Enabled = copyToolStripMenuItem.Enabled = pasteToolStripMenuItem.Enabled = false;
                    imageToolStripMenuItem.Visible = filterToolStripMenuItem.Visible = false;
                    openInnewDocumentOnChangeToolStripMenuItem.Visible = rememberOnChangeToolStripMenuItem.Visible = false;
                    //Status Strip
                    PositionToolStripStatusLabel.Visible = zoomPanel.Visible = sizePanel.Visible = false;
                    break;
                default:
                    break;
            }
        }

        public void ChildFormDesigner(FormStyle FS)
        {
            switch (FS)
            {
                case FormStyle.PaintHandleForm:
                    if (ActiveIHF != null)
                    {
                        //Tool Strip
                        ToolStripButton04Save.Click -= new EventHandler(ActiveIHF.SaveFile_Click);
                        ToolStripButton05SaveAs.Click -= new EventHandler(ActiveIHF.SaveAsFile_Click);
                        ToolStripButton07Copy.Click -= new EventHandler(ActiveIHF.CopyShape_Click);
                        ToolStripButton08Past.Click -= new EventHandler(ActiveIHF.PastShape_Click);
                        //Menu Strip
                        saveToolStripMenuItem.Click -= new EventHandler(ActiveIHF.SaveFile_Click);
                        saveAsToolStripMenuItem.Click -= new EventHandler(ActiveIHF.SaveAsFile_Click);
                        copyToolStripMenuItem.Click -= new EventHandler(ActiveIHF.CopyShape_Click);
                        pasteToolStripMenuItem.Click -= new EventHandler(ActiveIHF.PastShape_Click);
                    }
                    //Tool Strip
                    ToolStripButton04Save.Click += new EventHandler(ActivePHF.SaveFile_Click);
                    ToolStripButton05SaveAs.Click += new EventHandler(ActivePHF.SaveAsFile_Click);
                    ToolStripButton07Copy.Click += new EventHandler(ActivePHF.CopyShape_Click);
                    ToolStripButton08Past.Click += new EventHandler(ActivePHF.PastShape_Click);
                    if (EnableActivePHF)
                    {
                        ToolStripButton11AntiAlias.Click += new EventHandler(ActivePHF.AntiAlias_Click);
                        ToolStripButton12ShowClipping.Click += new EventHandler(ActivePHF.ShowClippingRectangleIsOn_Click);
                        ToolStripMenuItem13DeleteShape.Click += new EventHandler(ActivePHF.DeleteShape_Click);
                        ToolStripMenuItem13Color.Click += new EventHandler(ActivePHF.ColorEditor_Click);
                        ToolStripMenuItem13Properties.Click += new EventHandler(ActivePHF.ShapesProperties_Click);
                        ToolStripButton14ConvertToCurve.Click += new EventHandler(ActivePHF.ConvertToCurve_Click);
                        ToolStripButton15ConvertToLine.Click += new EventHandler(ActivePHF.ConvertToLine_Click);
                        ToolStripButton16DeleteNode.Click += new EventHandler(ActivePHF.DeleteNode_Click);
                        ToolStripButton17SelectionTool.Click += new EventHandler(ActivePHF.SelectionTool_Click);
                        ToolStripButton18NodeTool.Click += new EventHandler(ActivePHF.NodeTool_Click);
                        ToolStripButton19BringToFront.Click += new EventHandler(ActivePHF.BringToFront_Click);
                        ToolStripButton20SendToBack.Click += new EventHandler(ActivePHF.SendToBack_Click);
                        //EnableActivePHF = false;
                    }
                    //Menu Strip
                    saveToolStripMenuItem.Click += new EventHandler(ActivePHF.SaveFile_Click);
                    saveAsToolStripMenuItem.Click += new EventHandler(ActivePHF.SaveAsFile_Click);
                    copyToolStripMenuItem.Click += new EventHandler(ActivePHF.CopyShape_Click);
                    pasteToolStripMenuItem.Click += new EventHandler(ActivePHF.PastShape_Click);
                    
                    break;
                case FormStyle.ImageHandleForm:
                    if (ActivePHF != null)
                    {
                        //Tool Strip
                        ToolStripButton04Save.Click -= new EventHandler(ActivePHF.SaveFile_Click);
                        ToolStripButton05SaveAs.Click -= new EventHandler(ActivePHF.SaveAsFile_Click);
                        ToolStripButton07Copy.Click -= new EventHandler(ActivePHF.CopyShape_Click);
                        ToolStripButton08Past.Click -= new EventHandler(ActivePHF.PastShape_Click);
                        //Menu Strip
                        saveToolStripMenuItem.Click -= new EventHandler(ActivePHF.SaveFile_Click);
                        saveAsToolStripMenuItem.Click -= new EventHandler(ActivePHF.SaveAsFile_Click);
                        copyToolStripMenuItem.Click -= new EventHandler(ActivePHF.CopyShape_Click);
                        pasteToolStripMenuItem.Click -= new EventHandler(ActivePHF.PastShape_Click);
                    }
                    //Tool Strip
                    ToolStripButton04Save.Click += new EventHandler(ActiveIHF.SaveFile_Click);
                    ToolStripButton05SaveAs.Click += new EventHandler(ActiveIHF.SaveAsFile_Click);
                    ToolStripButton07Copy.Click += new EventHandler(ActiveIHF.CopyShape_Click);
                    ToolStripButton08Past.Click += new EventHandler(ActiveIHF.PastShape_Click);
                    //Menu Strip
                    saveToolStripMenuItem.Click += new EventHandler(ActiveIHF.SaveFile_Click);
                    saveAsToolStripMenuItem.Click += new EventHandler(ActiveIHF.SaveAsFile_Click);
                    copyToolStripMenuItem.Click += new EventHandler(ActiveIHF.CopyShape_Click);
                    pasteToolStripMenuItem.Click += new EventHandler(ActiveIHF.PastShape_Click);
                    if (EnableActiveIHF)
                    {
                        #region Menu Strip Image & Filter Triggers
                        //+ Menu Strip -> Image
                        backToolStripMenuItem.Click += new EventHandler(ActiveIHF.backImageItem_Click);
                        cloneToolStripMenuItem.Click += new EventHandler(ActiveIHF.cloneImageItem_Click);
                        //+ Menu Strip -> Image > Zoom
                        z10ToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomItem_Click);
                        z25ToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomItem_Click);
                        z50ToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomItem_Click);
                        z75ToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomItem_Click);
                        z100ToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomItem_Click);
                        z150ToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomItem_Click);
                        z200ToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomItem_Click);
                        z400ToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomItem_Click);
                        z500ToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomItem_Click);
                        zoomInToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomInImageItem_Click);
                        zoomOutToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomOutImageItem_Click);
                        fitToScreenToolStripMenuItem.Click += new EventHandler(ActiveIHF.zoomFitImageItem_Click);
                        flipToolStripMenuItem.Click += new EventHandler(ActiveIHF.flipImageItem_Click);
                        mirrorToolStripMenuItem.Click += new EventHandler(ActiveIHF.mirrorItem_Click);
                        rotate90DegreeToolStripMenuItem.Click += new EventHandler(ActiveIHF.rotateImageItem_Click);
                        cropToolStripMenuItem.Click += new EventHandler(ActiveIHF.cropImageItem_Click);
                        //+ Menu Strip -> Filter -> Color
                        grayscaleToolStripMenuItem.Click += new EventHandler(ActiveIHF.grayscaleColorFiltersItem_Click);
                        grayscaleToRGBToolStripMenuItem.Click += new EventHandler(ActiveIHF.toRgbColorFiltersItem_Click);
                        sepiaToolStripMenuItem.Click += new EventHandler(ActiveIHF.sepiaColorFiltersItem_Click);
                        invertToolStripMenuItem.Click += new EventHandler(ActiveIHF.invertColorFiltersItem_Click);
                        rotateToolStripMenuItem.Click += new EventHandler(ActiveIHF.rotateColorFiltersItem_Click);
                        colorFilteringToolStripMenuItem.Click += new EventHandler(ActiveIHF.colorFilteringColorFiltersItem_Click);
                        euclideanColorFilteringToolStripMenuItem.Click += new EventHandler(ActiveIHF.euclideanFilteringColorFiltersItem_Click);
                        channelsFilteringToolStripMenuItem.Click += new EventHandler(ActiveIHF.channelsFilteringColorFiltersItem_Click);
                        extractRedChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.extractRedColorFiltersItem_Click);
                        extractGreenChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.extractGreenColorFiltersItem_Click);
                        extractBlueChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.extractRedBlueFiltersItem_Click);
                        replaceRedChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.replaceRedColorFiltersItem_Click);
                        replaceGreenChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.replaceGreenColorFiltersItem_Click);
                        replaceBlueChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.replaceBlueColorFiltersItem_Click);
                        redToolStripMenuItem.Click += new EventHandler(ActiveIHF.redColorFiltersItem_Click);
                        greenToolStripMenuItem.Click += new EventHandler(ActiveIHF.greenColorFiltersItem_Click);
                        blueToolStripMenuItem.Click += new EventHandler(ActiveIHF.blueColorFiltersItem_Click);
                        cyanToolStripMenuItem.Click += new EventHandler(ActiveIHF.cyanColorFiltersItem_Click);
                        magentaToolStripMenuItem.Click += new EventHandler(ActiveIHF.magentaColorFiltersItem_Click);
                        yellowToolStripMenuItem.Click += new EventHandler(ActiveIHF.yellowColorFiltersItem_Click);
                        //+ Menu Strip -> Filter -> HSL Color Space
                        brightnessToolStripMenuItem.Click += new EventHandler(ActiveIHF.brightnessHslFiltersItem_Click);
                        contrastToolStripMenuItem.Click += new EventHandler(ActiveIHF.contrastHslFiltersItem_Click);
                        saturationToolStripMenuItem.Click += new EventHandler(ActiveIHF.saturationHslFiltersItem_Click);
                        hSLLinearToolStripMenuItem.Click += new EventHandler(ActiveIHF.linearHslFiltersItem_Click);
                        hSLFilteringToolStripMenuItem.Click += new EventHandler(ActiveIHF.filteringHslFiltersItem_Click);
                        hueModifierToolStripMenuItem.Click += new EventHandler(ActiveIHF.hueHslFiltersItem_Click);
                        //+ Menu Strip -> Filter -> YCbCr Color space
                        yCbCrLinearToolStripMenuItem.Click += new EventHandler(ActiveIHF.linearYCbCrFiltersItem_Click);
                        yCbCrFilteringToolStripMenuItem.Click += new EventHandler(ActiveIHF.filteringYCbCrFiltersItem_Click);
                        extractYChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.extracYFiltersItem_Click);
                        extractCbChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.extracCbFiltersItem_Click);
                        extractCrChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.extracCrFiltersItem_Click);
                        replaceYChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.replaceYFiltersItem_Click);
                        replaceCbChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.replaceCbFiltersItem_Click);
                        replaceCrChannelToolStripMenuItem.Click += new EventHandler(ActiveIHF.replaceCrFiltersItem_Click);
                        //+ Menu Strip -> Filter -> Binarization
                        thresholdToolStripMenuItem.Click += new EventHandler(ActiveIHF.thresholdBinaryFiltersItem_Click);
                        thresholdWithErrorCarryToolStripMenuItem.Click += new EventHandler(ActiveIHF.thresholdCarryBinaryFiltersItem_Click);
                        orderedDitherToolStripMenuItem.Click += new EventHandler(ActiveIHF.orderedDitherBinaryFiltersItem_Click);
                        bayerOrderedDitherToolStripMenuItem.Click += new EventHandler(ActiveIHF.bayerDitherBinaryFiltersItem_Click);
                        floydSteinbergToolStripMenuItem.Click += new EventHandler(ActiveIHF.floydBinaryFiltersItem_Click);
                        burkesToolStripMenuItem.Click += new EventHandler(ActiveIHF.burkesBinaryFiltersItem_Click);
                        stuckiToolStripMenuItem.Click += new EventHandler(ActiveIHF.stuckiBinaryFiltersItem_Click);
                        jarvisJudiceNinkeToolStripMenuItem.Click += new EventHandler(ActiveIHF.jarvisBinaryFiltersItem_Click);
                        sierraToolStripMenuItem.Click += new EventHandler(ActiveIHF.sierraBinaryFiltersItem_Click);
                        stevensonAndArceToolStripMenuItem.Click += new EventHandler(ActiveIHF.stevensonBinaryFiltersItem_Click);
                        sISThresholdToolStripMenuItem.Click += new EventHandler(ActiveIHF.sisThresholdBinaryFiltersItem_Click);
                        //+ Menu Strip -> Filter -> Morphology
                        erosionToolStripMenuItem.Click += new EventHandler(ActiveIHF.erosionMorphologyFiltersItem_Click);
                        dilatationToolStripMenuItem.Click += new EventHandler(ActiveIHF.dilatationMorphologyFiltersItem_Click);
                        openingToolStripMenuItem.Click += new EventHandler(ActiveIHF.openingMorphologyFiltersItem_Click);
                        closingToolStripMenuItem.Click += new EventHandler(ActiveIHF.closingMorphologyFiltersItem_Click);
                        customToolStripMenuItem.Click += new EventHandler(ActiveIHF.customMorphologyFiltersItem_Click);
                        hitAndMissThickeningThinningToolStripMenuItem.Click += new EventHandler(ActiveIHF.hitAndMissFiltersItem_Click);
                        //+ Menu Strip -> Filter -> Convolution & Correlation
                        meanToolStripMenuItem.Click += new EventHandler(ActiveIHF.meanConvolutionFiltersItem_Click);
                        blurToolStripMenuItem.Click += new EventHandler(ActiveIHF.blurConvolutionFiltersItem_Click);
                        sharpenToolStripMenuItem.Click += new EventHandler(ActiveIHF.sharpenConvolutionFiltersItem_Click);
                        edgesToolStripMenuItem.Click += new EventHandler(ActiveIHF.edgesConvolutionFiltersItem_Click);
                        customToolStripMenuItem1.Click += new EventHandler(ActiveIHF.customConvolutionFiltersItem_Click);
                        gaussianToolStripMenuItem.Click += new EventHandler(ActiveIHF.gaussianConvolutionFiltersItem_Click);
                        sharpenExToolStripMenuItem.Click += new EventHandler(ActiveIHF.sharpenExConvolutionFiltersItem_Click);
                        //+ Menu Strip -> Filter -> Two source filter
                        mergeToolStripMenuItem.Click += new EventHandler(ActiveIHF.mergeTwosrcFiltersItem_Click);
                        intersectToolStripMenuItem.Click += new EventHandler(ActiveIHF.intersectTwosrcFiltersItem_Click);
                        addToolStripMenuItem.Click += new EventHandler(ActiveIHF.addTwosrcFiltersItem_Click);
                        subtractToolStripMenuItem.Click += new EventHandler(ActiveIHF.subtractTwosrcFiltersItem_Click);
                        differenceToolStripMenuItem.Click += new EventHandler(ActiveIHF.differenceTwosrcFiltersItem_Click);
                        moveTowardsToolStripMenuItem.Click += new EventHandler(ActiveIHF.moveTowardsTwosrcFiltersItem_Click);
                        morphToolStripMenuItem.Click += new EventHandler(ActiveIHF.morphTwosrcFiltersItem_Click);
                        //+ Menu Strip -> Filter -> Edge detectors 
                        homogenityToolStripMenuItem.Click += new EventHandler(ActiveIHF.homogenityEdgeFiltersItem_Click);
                        differenceToolStripMenuItem1.Click += new EventHandler(ActiveIHF.differenceEdgeFiltersItem_Click);
                        sobelToolStripMenuItem.Click += new EventHandler(ActiveIHF.sobelEdgeFiltersItem_Click);
                        cannyToolStripMenuItem.Click += new EventHandler(ActiveIHF.cannyEdgeFiltersItem_Click);
                        //+ Menu Strip -> Filter -> Other
                        adaptiveSmoothingToolStripMenuItem.Click += new EventHandler(ActiveIHF.adaptiveSmoothingFiltersItem_Click);
                        conservativeSmoothingToolStripMenuItem.Click += new EventHandler(ActiveIHF.conservativeSmoothingFiltersItem_Click);
                        perlinNoiseToolStripMenuItem.Click += new EventHandler(ActiveIHF.perlinNoiseFiltersItem_Click);
                        oilPaintingToolStripMenuItem.Click += new EventHandler(ActiveIHF.oilPaintingFiltersItem_Click);
                        jitterToolStripMenuItem.Click += new EventHandler(ActiveIHF.jitterFiltersItem_Click);
                        pixellateToolStripMenuItem.Click += new EventHandler(ActiveIHF.pixellateFiltersItem_Click);
                        simpleSkeletonizationToolStripMenuItem.Click += new EventHandler(ActiveIHF.simpleSkeletonizationFiltersItem_Click);
                        shrinkToolStripMenuItem.Click += new EventHandler(ActiveIHF.shrinkFiltersItem_Click);
                        connectedComponentsLabelingToolStripMenuItem.Click += new EventHandler(ActiveIHF.labelingFiltersItem_Click);
                        blobExtractorToolStripMenuItem.Click += new EventHandler(ActiveIHF.blobExtractorFiltersItem_Click);
                        //+ Menu Strip -> Filter
                        resizeToolStripMenuItem.Click += new EventHandler(ActiveIHF.resizeFiltersItem_Click);
                        rotateToolStripMenuItem1.Click += new EventHandler(ActiveIHF.rotateFiltersItem_Click);
                        levelsToolStripMenuItem.Click += new EventHandler(ActiveIHF.levelsFiltersItem_Click);
                        medianToolStripMenuItem.Click += new EventHandler(ActiveIHF.medianFiltersItem_Click);
                        gammaCorrectionToolStripMenuItem.Click += new EventHandler(ActiveIHF.gammaFiltersItem_Click);
                        //fourierTransformationToolStripMenuItem.Click += new EventHandler(ActiveIHF.fourierFiltersItem_Click);
                        imageToolStripMenuItem.DropDownOpening += new EventHandler(ActiveIHF.imageItem_Popup);
                        #endregion
                        SetupDocumentEvents(ActiveIHF);
                        EnableActiveIHF = false;
                    }
                    break;
                default:
                    break;
            }
        }

        public void NewFile_Click(object sender, EventArgs e)
        {
            try
            {
                NewPaintForm NPF = new NewPaintForm(this);
                NPF.ShowDialog(this);
            }
            catch (Exception E) { MessageBox.Show(E.Message, "Error File", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public void OpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog OFD = new OpenFileDialog();
                OFD.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                OFD.Filter = "Photo Brush Project File (*.pbp)|*.pbp|Image File (*.png;*.jpg;*.tif;*.bmp;*.gif)|*.png;*.jpg;*.tif;*.bmp;*.gif|Adobe Photoshop Image (*.psd)|*.psd|All Files (*.*)|*.*";
                
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    switch(OFD.SafeFileName.Substring(OFD.SafeFileName.IndexOf('.')))
                    {
                        case ".png":
                        case ".jpg":
                        case ".tif":
                        case ".bmp":
                        case ".gif":
                        ImageHandlerForm IHF = new ImageHandlerForm(OFD.FileName, this);
                        IHF.Text = OFD.SafeFileName;//OFD.FileName.Remove(0, OFD.InitialDirectory.Length + 1);
                        //IHF.pictureBox1.Image = Image.FromFile(OFD.FileName);
                        Initialize(IHF, FormStyle.ImageHandleForm);
                        SetupDocumentEvents(IHF);
                        break;
                        case ".pbp":
                        PaintHandlerForm PHF = new PaintHandlerForm(this);
                        PHF.Text = OFD.SafeFileName;//OFD.FileName.Remove(0, OFD.InitialDirectory.Length + 1);
                        PHF.drawingCanvas1.shapeManager.FileName = OFD.FileName;
                        using (FileStream FS = new FileStream(OFD.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            BinaryFormatter BF = new BinaryFormatter();
                            ShapeManager SM = (ShapeManager)BF.Deserialize(FS);
                            SM.RestoreNonSerializable(PHF.drawingCanvas1);
                            PHF.drawingCanvas1.shapeManager = SM;
                        }
                        PHF.drawingCanvas1.Invalidate();
                        Initialize(PHF, FormStyle.PaintHandleForm);
                        break;
                        case ".psd":
                        PsdHandlerForm PsdHF = new PsdHandlerForm();
                        PsdHF.Text = OFD.SafeFileName;
                        PsdHF.OpenPsdFile(OFD.FileName);
                        Initialize(PsdHF, FormStyle.PsdHandlerForm);
                        break;
                        default:
                        ReadyToolStripStatusLabel.Text = "Sorry cannot open this file !";
                        break;
                    }
                }
            }
            catch (Exception E) { MessageBox.Show(E.Message, "Error File", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void toolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip1.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }
        // On "Options" popup
        private void optionsItem_Popup(object sender, System.EventArgs e)
        {
            this.openInnewDocumentOnChangeToolStripMenuItem.Checked = config.openInNewDoc;
            this.rememberOnChangeToolStripMenuItem.Checked = config.rememberOnChange;
        }
        // On "Options->Open in new Document" click
        private void openInNewOptionsItem_Click(object sender, System.EventArgs e)
        {
            config.openInNewDoc = !config.openInNewDoc;
        }
        // On "Options->Remember on change" click
        private void rememberOptionsItem_Click(object sender, System.EventArgs e)
        {
            config.rememberOnChange = !config.rememberOnChange;
        }

        #region IDocumentsHost implementation
        // Create new document on change on existent or modify it
        public bool CreateNewDocumentOnChange
        {
            get { return config.openInNewDoc; }
        }

        // Remember or not an image, so we can back one step
        public bool RememberOnChange
        {
            get { return config.rememberOnChange; }
        }

        // Create new document
        public bool NewDocument(Bitmap image)
        {
            unnamedNumber++;
            ImageHandlerForm IHF2 = new ImageHandlerForm(image, (IDocumentsHost)this);
            //IHF2.Image = image;
            IHF2.Text = ActiveIHF.Text +" "+ unnamedNumber.ToString();
            IHF2.SafeFileName = ActiveIHF.SafeFileName;
            Initialize(IHF2, FormStyle.ImageHandleForm);
            //SetupDocumentEvents(IHF2);
            return true;
        }

        // Create new document for ComplexImage
        public bool NewDocument(ComplexImage image)
        {
            unnamedNumber++;

            //FourierDoc imgDoc = new FourierDoc(image, (IDocumentsHost)this);

            //imgDoc.Text = "Image " + unnamedNumber.ToString();
            //imgDoc.Show(dockManager);
            //imgDoc.Focus();

            return true;
        }

        // Get an image with specified dimension and pixel format
        public Bitmap GetImage(object sender, String text, Size size, PixelFormat format)
        {
            ArrayList names = new ArrayList();
            ArrayList images = new ArrayList();
            foreach (ImageHandlerForm item in IHFList)
            {
                if ((item != sender) && (item is ImageHandlerForm))
                {
                    Bitmap img =(Bitmap) item.Image;
                    // check pixel format, width and height
                    if ((img.PixelFormat == format) &&
                        ((size.Width == -1) ||
                        ((img.Width == size.Width) && (img.Height == size.Height))))
                    {
                        names.Add(item.Text);
                        images.Add(img);
                    }
                }
            }

            SelectImageForm SIF = new SelectImageForm();
            SIF.Description = text;
            SIF.ImageNames = names;
            // allow user to select an image
            if ((SIF.ShowDialog() == DialogResult.OK) && (SIF.SelectedItem != -1))
            {
                return (Bitmap)images[SIF.SelectedItem];
            }
            return null;
        }
        #endregion
        // Setup events
        private void SetupDocumentEvents(ImageHandlerForm ActivatedIHF)
        {
            ActivatedIHF.DocumentChanged += new System.EventHandler(this.document_DocumentChanged);
            ActivatedIHF.ZoomChanged += new System.EventHandler(this.document_ZoomChanged);
            ActivatedIHF.MouseImagePosition += new ImageHandlerForm.SelectionEventHandler(this.document_MouseImagePosition);
            ActivatedIHF.SelectionChanged += new ImageHandlerForm.SelectionEventHandler(this.document_SelectionChanged);
            ActivatedIHF.MouseHover += new EventHandler(this.dockManager_ActiveDocumentChanged);
        }
        // active document changed
        private void dockManager_ActiveDocumentChanged(object sender, System.EventArgs e)
        {
            ImageHandlerForm doc = ActiveIHF;
            ImageHandlerForm imgDoc = (doc is ImageHandlerForm) ? (ImageHandlerForm)doc : null;

            UpdateHistogram(imgDoc);
            UpdateStatistics(imgDoc);
            UpdateZoomStatus(imgDoc);

            UpdateSizeStatus(doc);
        }
        // On document changed
        private void document_DocumentChanged(object sender, System.EventArgs e)
        {
            UpdateHistogram((ImageHandlerForm)sender);
            UpdateStatistics((ImageHandlerForm)sender);
            UpdateSizeStatus((ImageHandlerForm)sender);
        }
        // On zoom factor changed
        private void document_ZoomChanged(object sender, System.EventArgs e)
        {
            UpdateZoomStatus((ImageHandlerForm)sender);
        }

        // On mouse position over image changed
        private void document_MouseImagePosition(object sender, SelectionEventArgs e)
        {
            if (e.Location.X >= 0)
            {
                this.PositionToolStripStatusLabel.Text = string.Format(" ({0}, {1})", e.Location.X, e.Location.Y);

                // get current color
                Bitmap image = ((ImageHandlerForm)sender).Image;
                if (image.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Color color = image.GetPixel(e.Location.X, e.Location.Y);
                    RGB rgb = new RGB(color);
                    YCbCr ycbcr = new YCbCr();

                    AForge.Imaging.ColorConverter.RGB2YCbCr(rgb, ycbcr);

                    // RGB
                    this.colorPanel.Text = string.Format("RGB: {0}, {1}, {2}", color.R, color.G, color.B);
                    // HSL
                    this.hslPanel.Text = string.Format("HSL: {0}, {1:F3}, {2:F3}", (int)color.GetHue(), color.GetSaturation(), color.GetBrightness());
                    // YCbCr
                    this.ycbcrPanel.Text = string.Format("YCbCr: {0:F3}, {1:F3}, {2:F3}", ycbcr.Y, ycbcr.Cb, ycbcr.Cr);
                }
                else if (image.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Color color = image.GetPixel(e.Location.X, e.Location.Y);
                    this.colorPanel.Text = "Gray: " + color.R.ToString();
                    this.hslPanel.Text = "";
                    this.ycbcrPanel.Text = "";
                }
            }
            else
            {
                this.PositionToolStripStatusLabel.Text = "";
                this.colorPanel.Text = "";
                this.hslPanel.Text = "";
                this.ycbcrPanel.Text = "";
            }
        }

        // On selection changed
        private void document_SelectionChanged(object sender, SelectionEventArgs e)
        {
            this.PositionToolStripStatusLabel.Text = string.Format("({0}, {1}) - {2} x {3}", e.Location.X, e.Location.Y, e.Size.Width, e.Size.Height);
        }

        // Update histogram
        private void UpdateHistogram(ImageHandlerForm imgDoc)
        {
            if (HF.Visible)
                HF.GatherStatistics((imgDoc == null) ? null : imgDoc.Image);
        }

        private void UpdateStatistics(ImageHandlerForm imgDoc)
        {
            if (DF.Visible)
                DF.GatherStatistics((imgDoc == null) ? null : imgDoc.Image);
        }

        // Update size status
        private void UpdateSizeStatus(ImageHandlerForm doc)
        {
            if (doc != null)
            {
                int w = 0, h = 0;

                if (doc is ImageHandlerForm)
                {
                    w = ((ImageHandlerForm)doc).ImageWidth;
                    h = ((ImageHandlerForm)doc).ImageHeight;
                }
                //else if (doc is FourierDoc)
                //{
                //    w = ((FourierDoc)doc).ImageWidth;
                //    h = ((FourierDoc)doc).ImageHeight;
                //}

                sizePanel.Text = "SIZE " + w.ToString() + " x " + h.ToString();
            }
            else
            {
                sizePanel.Text = String.Empty;
            }
        }

        // Update zoom status
        private void UpdateZoomStatus(ImageHandlerForm imgDoc)
        {
            if (imgDoc != null)
            {
                int zoom = (int)(imgDoc.Zoom * 100);
                zoomPanel.Text = "ZOOM "+ zoom.ToString() + "%";
            }
            else
            {
                zoomPanel.Text = String.Empty;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // save configuration
			config.mainWindowLocation = this.Location;
			config.mainWindowSize = this.Size;
			config.Save(configFile);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Forms_Builder.AboutPB PB = new Forms_Builder.AboutPB();
            PB.ShowDialog();
        }
    }
}
