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
      steps {
        ansiblePlaybook(
          playbook: '/storage/api_deploy.yaml',
          credentialsId: 'ansible_key',
          inventory: '/storage/hosts',
          disableHostKeyChecking: true
        )
      }
    }
  }

  post{
      always{
          cleanWs()
      }
  }
}