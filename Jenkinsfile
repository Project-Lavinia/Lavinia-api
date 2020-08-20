pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh 'dotnet restore'
        sh 'dotnet build --configuration Release'
      }
    }

    stage('Deploy') {
      steps([$class: 'BapSshPromotionPublisherPlugin']) {
        script {
                def props = readProperties file: '/storage/jenkins_vars.properties'
                env.NETCORE_PATH = props.NETCORE_PATH
            }
        sshPublisher(
            continueOnError: false, failOnError: true,
            publishers: [
                sshPublisherDesc(
                    configName: "api-0",
                    verbose: true,
                    transfers: [
                        sshTransfer(execCommand: "sudo /bin/rm -rf ${NETCORE_PATH}/*"),
                        sshTransfer(sourceFiles: "Lavinia-api/bin/Release/netcoreapp3.1/**/*"),
                        /*
                          * Move and remove must happen in two stages because the root directory Lavinia-api
                          * conflicts with a filename in netcoreapp3.1
                          */
                        sshTransfer(execCommand: "mv ${NETCORE_PATH}/Lavinia-api/bin/Release/* ${NETCORE_PATH}/"),
                        sshTransfer(execCommand: "rm -r ${NETCORE_PATH}/Lavinia-api"),
                        sshTransfer(execCommand: "mv ${NETCORE_PATH}/netcoreapp3.1/* ${NETCORE_PATH}/"),
                        sshTransfer(execCommand: "rm -r ${NETCORE_PATH}/netcoreapp3.1"),
                        sshTransfer(execCommand: "sudo chmod -R 0755 ${NETCORE_PATH}"),
                        sshTransfer(execCommand: "sudo systemctl restart api")
                    ],
                )
            ]
        )
      }
    }
  }
}