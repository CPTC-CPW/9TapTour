using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Helpers
{
    /// <summary>
    /// WinForms control helpers. The pure validation half of the old
    /// FormHelper lives in NineTapTour.Core.Calculations.ValidationHelper.
    /// </summary>
    public static class FormHelper
    {
        /// <summary>
		/// Sets the flow direction for the flowlayoutpanel depending
		/// on the pixel width or height of the screen.
        /// </summary>
        /// <param name="form">The form object that calls this method.</param>
        /// <param name="flp">The flowlayoutpanel that is being passed in to have changes made to it.</param>
        /// <param name="width">The maximum width at which the flow direction is changed to top down</param>
        /// <param name="height">The minimum height at which the flow direction is changed to top down</param>
        public static void SetFlowDirection(Form form, FlowLayoutPanel flp, int width, int height)
        {
            if (form.Size.Width > width && form.Size.Height < height)
            {
                flp.FlowDirection = FlowDirection.TopDown;
            }
            else
            {
                flp.FlowDirection = FlowDirection.LeftToRight;
            }
        }

        /// <summary>
        /// Sets whether the scroll bars are enabled or disabled
        /// and whether you can see them or not depending on the
        /// pixel width or height of the screen.
        /// </summary>
        /// <param name="form">The form object that calls this method</param>
        /// <param name="flp">The flowlayoutpanel that is being passed in to have changes made to it.</param>
        /// <param name="width">The width at which horizontal scroll bars are toggled.</param>
        /// <param name="height">The height at which vertical scroll bars are toggled.</param>
        public static void SetFlowControlScrollBars(Form form, FlowLayoutPanel flp, int width, int height)
        {

            if (form.Size.Width < width)
            {
                flp.HorizontalScroll.Visible = true;
                flp.HorizontalScroll.Enabled = true;
            }
            else
            {
                flp.HorizontalScroll.Visible = false;
                flp.HorizontalScroll.Enabled = false;
            }

            if (form.Size.Height < height)
            {
                flp.VerticalScroll.Visible = true;
                flp.VerticalScroll.Enabled = true;
            }
            else
            {
                flp.VerticalScroll.Visible = false;
                flp.VerticalScroll.Enabled = false;
            }
        }

        /// <summary>
        /// check for empty text box
        /// </summary>
        public static bool IsEmpty(TextBox box)
        {
            if (string.IsNullOrEmpty(box.Text.Trim()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Selects last index of inputted value of the textbox
        /// </summary>
        /// <param name="txtBox">TextBox to be passed in</param>
        public static void GoToFirstIndexInTextboxIfEmpty(TextBoxBase txtBox)
        {
            if (txtBox.Text.StartsWith(" ") || txtBox.Text.StartsWith("( ") || txtBox.Text == string.Empty)
            {
                txtBox.Select(0, 0);
            }
        }

        /// <summary>
        /// Changes all radioButtons in the given groupbox with the given squadNumber to true
        /// </summary>
        public static void SelectParticipantSquad(int squadNumber, GroupBox groupBox)
        {
            foreach (Control control in groupBox.Controls)
            {
                RadioButton rdoButton = control as RadioButton;
                if (rdoButton.Text.Contains(squadNumber.ToString()))
                {
                    rdoButton.Checked = true;
                }
            }
        }

        /// <summary>
        /// This method gets the values for the Filter series radio buttons
        /// and returns a list of booleans that correspond to the squads chosen.
        /// Index 0 is All Squads
        /// </summary>
        /// <param name="groupBox">Specifically, the Filter Series box, GRPQBS1n on FrmMemberScores </param>
        public static List<bool> GetFilterSeriesList(GroupBox groupBox)
        {
            List<bool> filterSeries = [];
            foreach (Control control in groupBox.Controls)
            {
                CheckBox check = control as CheckBox;
                filterSeries.Add(check.Checked);
            }
            filterSeries.Reverse();
            return filterSeries;
        }

        /// <summary>
        /// This method takes a color hex code such as #000000 and
        /// returns a color to be used with the forms.
        /// </summary>
        /// <param name="hex">The color hex code starting with (#)</param>
        public static Color GenerateCustomColorFromHex(string hex)
        {
            return ColorTranslator.FromHtml(hex);
        }
    }
}
