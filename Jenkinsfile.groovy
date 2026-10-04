pipeline {
    agent any

    stages {
        stage('Restore') {
            steps {
                dir('backend') {
                    bat 'dotnet restore'
                }
            }
        }

        stage('SAST - SonarQube') {
            steps {
                dir('backend') {
                    withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
                        bat '''
                            java -version
                            dotnet tool update dotnet-sonarscanner --tool-path ..\\.sonar-tools
                            ..\\.sonar-tools\\dotnet-sonarscanner.exe begin /k:"PlataformAPI" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="%SONAR_TOKEN%"
                            dotnet build --no-restore -c Release
                            ..\\.sonar-tools\\dotnet-sonarscanner.exe end /d:sonar.token="%SONAR_TOKEN%"
                        '''
                    }
                }
            }
        }

        stage('Dependências vulneráveis') {
            steps {
                dir('backend') {
                    bat 'dotnet list package --vulnerable --include-transitive'
                }
            }
        }
    }
}