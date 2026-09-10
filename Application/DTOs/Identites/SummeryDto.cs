namespace Application.DTOs;
public class SummeryDto
{
    public int Branch {get; set; }
    public int Booth { get; set; }
    public int ActiveBooth { get; set; }
    public int ErrorBooth { get; set; }
    public decimal Revenue { get; set; }
}