using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineTapTour.Database
{
    public static class FormHelper
    {
        public static bool IsDateTimeTextBoxValid(TextBoxBase box)
        {
            if (DateTime.TryParse(box.Text, out DateTime dateTime))
            {
                if (dateTime >= DateTime.Parse("01/01/1753"))
                {
                    return true; 
                }
            }
            return false;
        }

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
        /// <param name="form">The form obejct that calls this method</param>
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
        /// 
        /// </summary>
        /// <param name="squadNumber"></param>
        /// <param name="groupBox"></param>
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
    }
}
