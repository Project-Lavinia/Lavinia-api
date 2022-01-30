def testPassed = true
def configuration = "Release"

pipeline {
  agent any
  environment {
    ARTIFACT = "artifact.zip"
  }
  stages {
    stage('Build') {
      def outputDirectory = "output"
      steps {
        sh "dotnet restore"
        sh "dotnet build --configuration ${configuration} --output ${outputDirectory}"
        sh "zip -r ${WORKSPACE}/${ARTIFACT} ${outputDirectory}/*"
        archiveArtifacts artifacts: ARTIFACT
      }
    }
    
    stage('Test') {
      steps {
        script {
          try {
            sh "dotnet test --configuration ${configuration} --no-restore --logger \"trx;LogFileName=TestResults.trx\""
          } catch (ex) {
            unstable('Some tests failed')
            testPassed = false
          }
        }
        mstest testResultsFile:"**/*.trx", keepLongStdio: true
      }
    }

    stage('Deploy') {
      when {
        allOf {
          tag "*.*.*"
          expression { testPassed }
        }
      }
      steps {
        ansiblePlaybook(
          playbook: '/storage/api_deploy.yaml',
          credentialsId: 'ansible_key',
          inventory: '/storage/hosts'
        )
      }
    }

    stage('Release') {
      when {
        allOf {
            tag "*.*.*"
            expression { testPassed }
          }
      }
      environment {
        GITHUB_TOKEN = credentials('jenkins_release_token')
        REPOSITORY = "Project-Lavinia/Lavinia-api"
      }
      steps {
        publishArtifact()
      }
    }
  }
}

void publishArtifact() {
    def current_tag = sh(returnStdout: true, script: "git tag --sort version:refname | tail -1").trim()
    def release_data = sh(returnStdout: true, script: "curl https://api.github.com/repos/${REPOSITORY}/releases/tags/${current_tag}").trim()
    def release_json = readJSON text: release_data
    def release_id = release_json.id
    sh "curl -XPOST -H 'Authorization:token ${GITHUB_TOKEN}' -H 'Content-Type:application/octet-stream' --data-binary @${ARTIFACT} https://uploads.github.com/repos/${REPOSITORY}/releases/${release_id}/assets?name=${ARTIFACT}"
}