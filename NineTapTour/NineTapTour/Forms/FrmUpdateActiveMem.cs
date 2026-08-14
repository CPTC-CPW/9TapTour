using NineTapTour.Database;
using NineTapTour.Core.Data;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NineTapTour.Forms;

public partial class FrmUpdateActiveMem : Form
{
    private readonly IMemberRepository memberRepository;
    private readonly IDbContextFactory<NineTapDb> dbFactory;

    DateTime targetDate;
    readonly List<Member> InActiveList;
    readonly List<Member> AllMembers;
    public FrmUpdateActiveMem(IMemberRepository memberRepository, IDbContextFactory<NineTapDb> dbFactory)
    {
        this.memberRepository = memberRepository;
        this.dbFactory = dbFactory;

        InitializeComponent();
        dateTimePicker1.Value = DateTime.Today.AddDays(-180);
        targetDate = dateTimePicker1.Value;
        InActiveList = memberRepository.GetMemberList();
        AllMembers = memberRepository.GetMemberList();
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

        var db = dbFactory.CreateDbContext();
        if (MessageBox.Show("Update the selected Members to inactive?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
        {
          
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
