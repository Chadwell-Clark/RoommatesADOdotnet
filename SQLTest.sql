Select r.FirstName, Count(c.Name) as Count
From Roommate r
Left JOIN RoommateChore rc on rc.RoommateId = r.Id
Left Join Chore c on rc.ChoreId = c.Id
Group By r.FirstName, r.Id;