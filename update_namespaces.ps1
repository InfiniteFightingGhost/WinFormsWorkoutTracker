$prefixes = "Controller", "RealView", "Tests", "Data", "Service", "Utility", "View"
Get-ChildItem -Filter *.cs -Recurse | ForEach-Object {
    $path = $_.FullName
    $content = [System.IO.File]::ReadAllText($path)
    $original = $content
    
    foreach ($p in $prefixes) {
        $content = [regex]::Replace($content, "(?m)^namespace\s+$p(\.|\s+|;|{|$)", "namespace WorkoutTracker.$p`$1")
        $content = [regex]::Replace($content, "(?m)^using\s+$p(\.|;)", "using WorkoutTracker.$p`$1")
    }
    
    $content = [regex]::Replace($content, "(?m)^namespace\s+LazyCrudProjectTemplate(\.|\s+|;|{|$)", "namespace WorkoutTracker.TestingProject`$1")
    $content = [regex]::Replace($content, "(?m)^using\s+LazyCrudProjectTemplate(\.|;)", "using WorkoutTracker.TestingProject`$1")

    if ($original -ne $content) {
        [System.IO.File]::WriteAllText($path, $content)
    }
}
