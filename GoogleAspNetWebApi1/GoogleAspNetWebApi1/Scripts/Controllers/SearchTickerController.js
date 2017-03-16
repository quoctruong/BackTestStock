var SearchTickerController = function ($scope, $http, ChartService) {
    var link = 'stock/Tickers';
    $http.get(link).then(function (response) {
        $scope.tickers = response.data;
    }, function (response) {
        console.log(response);
    });
    $scope.selectedTicker = '';
    $scope.submit = function () {
        if ($scope.selectedTicker) {
            console.log($scope.selectedTicker);
            ChartService.drawChart($scope.selectedTicker);
        }
    }
};

SearchTickerController.$inject = ['$scope', '$http', 'ChartService']
