google.charts.load('current', { 'packages': ['corechart'] });
google.charts.setOnLoadCallback(function () {
    angular.bootstrap(document.body, ['ChartApp'])
});
console.log('Blah');

function createTooltipHtml(low, open, close, high) {
    return '<div style="display: inline-block; width: 70px">' +
        '<span>Low: ' + low + '</span><br/>' +
        '<span>Open: ' + open + '</span><br/>' +
        '<span>Close: ' + close + '</span><br/>' +
        '<span>High: ' + high + '</span></div>';
}

var ChartController = function ($scope, $http) {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Date');
    data.addColumn('number', 'Low');
    data.addColumn('number', 'Opening');
    data.addColumn('number', 'Closing');
    data.addColumn('number', 'High');
    data.addColumn({ type: 'string', role: 'tooltip', 'p': { 'html': true } });
    data.addRows([
    ['Monnn', 20, 28, 38, 45, createTooltipHtml(20, 28, 38, 45)],
    ['Tuesss', 31, 55, 40, 66, createTooltipHtml(31, 55, 40, 66)],
    ['Wed', 50, 55, 77, 80, createTooltipHtml(50, 55, 77, 80)],
    ['Thu', 50, 77, 66, 77, createTooltipHtml(50, 77, 66, 77)],
    ['Fri', 15, 66, 22, 68, createTooltipHtml(15, 66, 22, 68)]]);
    var options = {
        seriesType: 'candlesticks',
        legend: 'none',
        candlestick: {
            hollowIsRising: true
        },
        tooltip: { isHtml: true }
    };
    var chart = new google.visualization.CandlestickChart(document.getElementById('chart_div'));
    var link = 'www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=MSFT&interval=1min&apikey=8118'
    $http.get(link).then(function (response) {
        console.log(response);
    }, function (response) {
        console.log(response);
    });
    chart.draw(data, options);

    $scope.chart = chart;

    /*
    $scope.getStockData = function () {
        $http.get()
             .success(function (data, status) {
                 $scope.response = data;
             }).error(function (data, status) {
                 $scope.response = 'Request failed';
             });
    }
    */
}

ChartController.$inject = ['$scope', '$http']
