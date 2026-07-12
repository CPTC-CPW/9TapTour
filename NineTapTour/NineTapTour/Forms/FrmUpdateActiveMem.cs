using NineTapTour.Database;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NineTapTour.Models;
using NineTapTour.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NineTapTour.Forms;

public partial class FrmUpdateActiveMem : Form
{
    DateTime targetDate;
    readonly List<Member> AllMembers;
    readonly IMemberRepository _memberRepo;
    readonly IDbContextFactory<NineTapDb> _dbFactory;

    public FrmUpdateActiveMem()
    {
        InitializeComponent();
    }

    [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
    public FrmUpdateActiveMem(IMemberRepository memberRepo, IDbContextFactory<NineTapDb> dbFactory)
    {
        InitializeComponent();
        _memberRepo = memberRepo;
        _dbFactory = dbFactory;
        dateTimePicker1.Value = DateTime.Today.AddDays(-180);
        targetDate = dateTimePicker1.Value;
        AllMembers = _memberRepo.GetMemberList();
        UpdateList();
    }

    private void UpdateList()
    {
        if (AllMembers == null)
        {
            MessageBox.Show("There are no members in the database");
            return;
        }
        AllMembers.Sort(new MemberNumComparer());

        InactiveListCheckBox.Sorted = false;

        foreach (var mem in AllMembers)
        {            
            // add members to the list
            if (mem.IsActive && (mem.LastBowled <= targetDate || mem.LastBowled.ToString() == ""))
            {
                InactiveListCheckBox.Items.Add(mem);
            }
        }
    }

    private void DateTimePicker1_ValueChanged(object sender, EventArgs e) => targetDate = dateTimePicker1.Value;

    private void BtnUpdateActive_Click(object sender, EventArgs e)
    {
        if (InactiveListCheckBox.CheckedItems.Count == 0)
        {
            MessageBox.Show("No members checked.");
            return;
        }

        if (MessageBox.Show("Update the selected Members to inactive?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
        {
            using var db = _dbFactory.CreateDbContext();
            foreach (Member mem in InactiveListCheckBox.CheckedItems)
            {
                mem.IsActive = false;
                db.Entry(mem).State = EntityState.Modified;
            }
            db.SaveChanges();
            InactiveListCheckBox.Items.Clear();
            UpdateList();
        }
    }

    private void BtnCheckInactive_Click(object sender, EventArgs e)
    {
        for (int i = 0; i < InactiveListCheckBox.Items.Count; i++)
        {
            InactiveListCheckBox.SetItemChecked(i, true);
        }
    }
}
