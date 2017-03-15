var ChartApp = angular.module('ChartApp', []);

google.charts.load('current', { 'packages': ['corechart'] });
google.charts.setOnLoadCallback(function () {
    angular.bootstrap(document.body, ['ChartApp'])
});

ChartApp.controller('HomePageController', HomePageController);
ChartApp.controller('SearchTickerController', SearchTickerController);
ChartApp.service('ChartService', ChartService);
