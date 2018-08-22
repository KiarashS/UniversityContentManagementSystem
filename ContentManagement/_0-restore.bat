rmdir /S /Q bin
rmdir /S /Q obj
dotnet restore
npm install -g yarn
yarn install
npm install
pause