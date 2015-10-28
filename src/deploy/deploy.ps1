properties {
  $solution_file = "Sms.sln"
}

task build {
  exec { msbuild $solution_file /t:Clean /t:Build /p:Configuration=Release /v:q }
}

task deploy -depends build {
  $delivery_directory = "C:\delivery"
  $executable = join-path $delivery_directory 'Sms.exe'
  
  if (test-path $delivery_directory) {
    exec { & $executable uninstall }
    rd $delivery_directory -rec -force  
  }
  
  copy-item 'Sms\bin\Release' $delivery_directory -force -recurse -verbose
  copy-item 'Sms\views' $delivery_directory -force -recurse -verbose
  copy-item 'Sms\static' $delivery_directory -force -recurse -verbose

  exec { & $executable install start }
}