# Batch convert all .xls files in a specified folder to .xlsx using Excel COM automation

param(
    [string]$folder = $null
)

if (-not $folder -or !(Test-Path $folder)) {
    Write-Host "No valid folder path provided. Exiting."
    exit 1
}

# Start Excel
$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$excel.DisplayAlerts = $false

# Get all .xls files (not .xlsx) in the specified folder
Get-ChildItem -Path $folder -Filter *.xls | Where-Object { $_.Extension -eq ".xls" } | ForEach-Object {
    $xlsPath = $_.FullName
    $xlsxPath = [System.IO.Path]::ChangeExtension($xlsPath, ".xlsx")
    Write-Host "Converting $xlsPath to $xlsxPath"
    $workbook = $excel.Workbooks.Open($xlsPath)
    $workbook.SaveAs($xlsxPath, 51) # 51 = xlOpenXMLWorkbook (xlsx)
    $workbook.Close($false)
}

# Quit Excel
$excel.Quit()
[System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null

Write-Host "All .xls files in the folder have been converted to .xlsx"
