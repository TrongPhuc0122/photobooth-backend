namespace Application.DTOs.Identites;
public class CreateSettingDto
{
    public required CameraDto Camera { get; set; }
    public required PrinterDto Printer { get; set; } 
}
public class CameraDto
{
    public string AE { get; set; } = string.Empty;
    public string ISO { get; set; } = string.Empty;
    public string Tv { get; set; } = string.Empty;             
    public string Av { get; set; } = string.Empty;             
    public string Exposure { get; set; } = string.Empty;    
    public string WB { get; set; } = string.Empty;            
    public string Metering { get; set; } = string.Empty;       
    public string Quality { get; set; } = string.Empty;       
    public string Flash { get; set; } = string.Empty;       
    public string AFMode { get; set; } = string.Empty;        
    public string PictureStyle { get; set; } = string.Empty; 
    public string DriveMode { get; set; } = string.Empty;      
}

public class PrinterDto
{
    public bool ColorCorrection { get; set; }
    public bool AutoRotate { get; set; }
    public bool Borderless { get; set; }      
    public bool HalfCut { get; set; }
    public string PrinterDriverName { get; set; } = string.Empty;
}