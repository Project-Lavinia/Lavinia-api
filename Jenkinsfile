pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh 'dotnet restore'
        sh 'dotnet build --configuration Release'
        sh 'ls -R'
      }
    }

    stage('Deploy') {
      when{
        branch 'master'
      }
      steps([$class: 'BapSshPromotionPublisherPlugin']) {
            sshPublisher(
                continueOnError: false, failOnError: true,
                publishers: [
                    sshPublisherDesc(
                        configName: "api-0",
                        verbose: true,
                        transfers: [
                            sshTransfer(execCommand: "sudo /bin/rm -rf /var/netcore/*"),
                            sshTransfer(sourceFiles: "Lavinia-api/bin/Release/netcoreapp3.1/**/*"),
                            sshTransfer(execCommand: "mv /var/netcore/netcoreapp3.1/* /var/netcore/"),
                            sshTransfer(execCommand: "rm -r /var/netcore/netcoreapp3.1"),
                            sshTransfer(execCommand: "sudo chmod -R 0755 /var/netcore"),
                            sshTransfer(execCommand: "sudo systemctl restart api")
                        ],
                    )
                ]
            )
        }
    }
  }
}