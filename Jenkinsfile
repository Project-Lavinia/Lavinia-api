pipeline {
  agent any
  stages {
    stage('Build') {
      steps {
        sh 'dotnet restore'
        sh 'dotnet build'
        sh 'ls'
      }
    }

  }
}