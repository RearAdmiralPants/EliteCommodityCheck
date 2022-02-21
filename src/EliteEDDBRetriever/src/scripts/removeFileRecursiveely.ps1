$target = $args[0];
$results = @();

$items = Get-ChildItem -Recurse ./ | Select-Object -ExpandProperty FullName
ForEach ($item in $items) {
    Write-Host "Checking $item for $target..."
    # if ($results.count -ne 1)
    $exists = Test-Path $item
    if ($exists) {
        $exists = $item.EndsWith($target)
    }
    If ($exists ) {
        Write-Host "Adding $result to results array."
        $results += $item;
    }
}

if ($results.count -ne 1) {
    Write-Host "Multiple files matched input. Doing nothing.";
    Write-Host "Matched items: ";
    foreach ($result in $results) {
        Write-Host "Matched: $result"
    }
}
else {    
    Write-Host "DELETING from local filesystem: " + $results[0];
    Remove-Item $results[0] -Force -Recurse
}
