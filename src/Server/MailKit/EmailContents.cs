
using System.Collections.Generic;
using System.Text;

namespace EctBlazorApp.Server.MailKit
{   
    public class EmailContents
    {
        public string TeamName { get; set; }
        public int PointsThreshold { get; set; }
        public double MarginDifference { get; set; }
        public List<NotificationMemberData> Members { get; set; }

        public override string ToString()
        {
            StringBuilder teamDataAsHtml = new("<!DOCTYPE html> <html lang=\"en\">"+
                $"<head> <meta charset=\"UTF-8\"> <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">"+
                $"<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">"+
                $"<title>ECT Email</title> </head> <body style=\"padding: 1em 3em;\">"+
                $"<h2 style=\"width: 100%; text-align: center;\">{TeamName}</h2>"+
                $"<p style=\"width: 100%; text-align: center;\">Isolated teammates</p>"+
                $"<div style=\"width: 100%; padding: 1em 0.5em\">"+
                $"<h3>Triggers</h3>"+
                $"<div style=\"width: 100%;\">"+
                $"<p style=\"width: max-content; height: 1em; padding: 0.2em 1em; border-bottom: 1px solid gray;\">Current total &lt; {PointsThreshold} points</p>"+
                $"<p style=\"width: max-content; height: 1em; padding: 0.2em 1em; border-bottom: 1px solid gray;\">Current total is {MarginDifference}% &lt; past total</p>"+
                $"</div>"+
                $"</div>");

            foreach (var member in Members)
            {
                teamDataAsHtml.Append(member);
            }

            return teamDataAsHtml.ToString();
        }
    }

    public class NotificationMemberData
    {
        public string Name { get; set; }
        public List<int> CurrentPoints { get; set; }
        public int CurrentTotal 
        {
            get
            {
                int total = 0;
                foreach (var currentPoint in CurrentPoints)
                    total += currentPoint;

                return total;
            }
        }
        public List<int> PastPoints { get; set; }
        public int PastTotal 
        {
            get
            {
                int total = 0;
                foreach (var pastPoint in PastPoints)
                    total += pastPoint;

                return total;
            }
        }
        public string CurrentWeek { get; set; } 
        public string PastWeek { get; set; } 

        public override string ToString()
        {
            StringBuilder memberDataAsHtml = new(
                $"<div style=\"width: 100%; margin-top: 1em; padding: 0.5em\">"+
                $"<h3 style=\"width: 100%;\">{Name}</h3>"+
                $"<div style=\"width: 40rem; padding: 1em; border: 1px solid gray;\">"+
                $"<table style=\"width: 80%;\">"+
                $"<thead>"+
                $"<tr> "+
                $"<td></td>"+
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none;\"> Mon </td>"+
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\"> Tue </td>"+
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\"> Wed </td>"+
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\"> Thu </td>"+
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\"> Fri </td>"+
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\"> Sat </td>"+
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\"> Sun </td>"+
                $"</tr> "+
                $"</thead>"+
                $"<tbody>"+
                $"<tr> "+
                $"<td>{PastWeek}</td> "+
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none;\">{PastPoints[0]}</td> ");

            for (int i = 1; i < PastPoints.Count; i++)
            {
                memberDataAsHtml.Append($"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\">{PastPoints[i]}</td>");
            }
            memberDataAsHtml.Append(
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\">"+
                $"<b>Total: {PastTotal}</b>"+
                $"</td>"+
                $"</tr>"+
                $"<tr>"+
                $"<td>{CurrentWeek}</td> "+
                $"<td style=\"text-align: center; border: 1px solid gray; border-top: none;\">{CurrentPoints[0]}</td>");
            
            for (int i = 1; i < CurrentPoints.Count; i++)
            {
                memberDataAsHtml.Append($"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\">{CurrentPoints[i]}</td>");
            }
            memberDataAsHtml.Append($"<td style=\"text-align: center; border: 1px solid gray; border-top: none; border-left: none;\">"+
                $"<b>Total: {CurrentTotal}</b>"+
                $"</tr>"+
                $"</tbody>"+
                $"</table>"+
                $"</div>");

            return memberDataAsHtml.ToString();
        }
    }
}