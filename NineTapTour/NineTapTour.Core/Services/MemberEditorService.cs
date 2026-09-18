using NineTapTour.Core.Calculations;
using NineTapTour.Core.Entities;
using NineTapTour.Core.Models;
using NineTapTour.Core.Repositories;

namespace NineTapTour.Core.Services;

public class MemberEditorService : IMemberEditorService
{
    private static readonly DateTime EarliestValidDate = new(1900, 1, 1);

    private readonly IMemberRepository memberRepository;
    private readonly IPlayerHistoryRepository playerHistoryRepository;
    private readonly IRegionRepository regionRepository;

    public MemberEditorService(IMemberRepository memberRepository, IPlayerHistoryRepository playerHistoryRepository,
        IRegionRepository regionRepository)
    {
        this.memberRepository = memberRepository;
        this.playerHistoryRepository = playerHistoryRepository;
        this.regionRepository = regionRepository;
    }

    public MemberEditModel Load(int memberNumber)
    {
        if (memberNumber <= 0)
        {
            memberNumber = memberRepository.GetLastMemberNumber() + 1;
        }

        Member member = memberRepository.GetMember(memberNumber);
        PlayerHistoryViewModel? mostRecent = playerHistoryRepository.GetMostRecentTournament(memberNumber);
        int thirtyGameAverage = mostRecent != null ? Convert.ToInt32(mostRecent.trueAVG) : 0;

        if (member.Id == 0)
        {
            Member blank = new()
            {
                Number = memberNumber,
                IsActive = true,
                RegionId = regionRepository.GetDefault().Id,
            };
            return new MemberEditModel { Member = blank, IsNew = true, ThirtyGameAverage = thirtyGameAverage };
        }

        // The desktop editor always shows the handicap implied by the average.
        member.Handicap = TournamentCalculations.CalculateHandicapPins(member.Average ?? 0);
        member.MoneyEarned = playerHistoryRepository.GetTotalMoneyWon(member.Number);

        string paidTo = member.LastPayment.HasValue && !member.IsLifetimeMember
            ? member.LastPayment.Value.AddYears(1).ToString("yyyy")
            : string.Empty;

        return new MemberEditModel
        {
            Member = member,
            IsNew = false,
            ThirtyGameAverage = thirtyGameAverage,
            PaidToYear = paidTo,
            IsPaymentDue = IsPaymentDue(member),
        };
    }

    public MemberNavigation GetNavigation(int memberNumber)
    {
        List<int> numbers = [.. memberRepository.GetMemberList().Select(m => m.Number).OrderBy(n => n)];
        if (numbers.Count == 0)
        {
            return new MemberNavigation(null, null, null, null, 0, 0);
        }

        int index = numbers.IndexOf(memberNumber);
        int? previous = index > 0 ? numbers[index - 1] : index < 0 ? numbers.LastOrDefault(n => n < memberNumber) : null;
        int? next = index >= 0 && index < numbers.Count - 1 ? numbers[index + 1] : index < 0 ? numbers.FirstOrDefault(n => n > memberNumber) : null;
        if (index < 0)
        {
            if (previous == 0) previous = null;
            if (next == 0) next = null;
        }

        return new MemberNavigation(numbers[0], previous, next, numbers[^1], index + 1, numbers.Count);
    }

    public MemberSaveResult Save(Member input)
    {
        List<string> errors = [];

        if (string.IsNullOrWhiteSpace(input.LastName))
        {
            errors.Add("Last name is required.");
        }
        if (string.IsNullOrWhiteSpace(input.FirstName))
        {
            errors.Add("First name is required.");
        }
        if (input.JoinDate == null || input.JoinDate < EarliestValidDate)
        {
            errors.Add("Date joined is required and must be after 1900.");
        }
        if (input.DateOfBirth.HasValue && input.DateOfBirth < EarliestValidDate)
        {
            errors.Add("Date of birth must be after 1900.");
        }

        int average = input.Average ?? 0;
        if (average < 0 || average > 300)
        {
            errors.Add("Average must be between 0 and 300.");
        }
        if (input.Bonus < 0 || input.Bonus > 5)
        {
            errors.Add("Bonus pins is invalid! Bonus must be between 0 and 5.");
        }

        if (errors.Count > 0)
        {
            return MemberSaveResult.Failed(errors);
        }

        // A member born 50 or more years ago is a senior, regardless of the checkbox.
        if (input.DateOfBirth.HasValue)
        {
            input.IsSenior = DateTime.Now.AddYears(-50) >= input.DateOfBirth.Value;
        }

        input.Average = average;
        input.Handicap = TournamentCalculations.CalculateHandicapPins(average);
        input.Referrals ??= 0;
        input.MiddleInitial ??= string.Empty;

        // Existing numbers update in place; anything else becomes the next free number.
        if (memberRepository.MemberExists(input))
        {
            input.Id = memberRepository.GetMemberIdByNumber(input.Number);
        }
        else
        {
            input.Id = 0;
            input.Number = memberRepository.GetLastMemberNumber() + 1;
        }

        // Money earned is always derived from finalized history, never typed.
        input.MoneyEarned = input.Id == 0 ? 0 : playerHistoryRepository.GetTotalMoneyWon(input.Number);

        memberRepository.AddOrUpdateMember(input);
        return MemberSaveResult.Ok(input);
    }

    public bool IsPaymentDue(Member member)
    {
        return !member.IsLifetimeMember
            && member.LastPayment.HasValue
            && member.LastPayment.Value < DateTime.Now.AddYears(-1);
    }
}
