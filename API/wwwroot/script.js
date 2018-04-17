angular.module("chatApp", [])
  .config(['$compileProvider', function ($compileProvider) {
      $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|local|data|chrome-extension):/);
  }])
  .controller("chatCtrl", ["$scope", "$timeout", ($scope, $timeout) => {
    $scope.name = "";
    $scope.key = "";
    $scope.initialized = false;
    $scope.activeUserCount = 0;
    $scope.blockchain = [
      {
        index: 0,
        name: "gensis_block",
        previousHash: "",
        date: new Date(),
        data: "gensis_message",
        hash: sha256.create().update(new Date().toString()).hex()
      }
    ];

    var crypto = (key) => {
      var hash = sha256.create();
      hash.update(key);
      key = hash.array();

      return {
        encrypt: (plaintext) => {
          // Convert text to bytes
          var text = plaintext;
          var textBytes = aesjs.utils.utf8.toBytes(text);

          // The counter is optional, and if omitted will begin at 1
          var aesCtr = new aesjs.ModeOfOperation.ctr(key, new aesjs.Counter(5));
          var encryptedBytes = aesCtr.encrypt(textBytes);

          // To print or store the binary data, you may convert it to hex
          var encryptedHex = aesjs.utils.hex.fromBytes(encryptedBytes);

          return encryptedHex;
        },
        decrypt: (ciphertext) => {
          // When ready to decrypt the hex string, convert it back to bytes
          var encryptedBytes = aesjs.utils.hex.toBytes(ciphertext);

          // The counter mode of operation maintains internal state, so to
          // decrypt a new instance must be instantiated.
          var aesCtr = new aesjs.ModeOfOperation.ctr(key, new aesjs.Counter(5));
          var decryptedBytes = aesCtr.decrypt(encryptedBytes);

          // Convert our bytes back into text
          var decryptedText = aesjs.utils.utf8.fromBytes(decryptedBytes);

          return decryptedText;
        }
      };
    };

    $scope.initialize = () => {
      $scope.initialized = true;
      $scope.crypto = crypto($scope.key)
      init();
    };

    var init = () => {
      $scope.messages = [];
      var conn = new signalR.HubConnection("./chat");

      conn.on("SendMessage", (blockchain, name, date) => {
        $scope.$apply(() => {
          $scope.blockchain = blockchain;

          // Add to the head of array
          $scope.messages.unshift({
            name: name,
            message: $scope.crypto.decrypt(blockchain[blockchain.length - 1].data),
            date: moment(date).format('h:mm:ss a'),
          });
        });
      });

      conn.on("SendAction", (data, activeUserCount) => {
        $scope.$apply(() => $scope.activeUserCount = activeUserCount );
      });

      $scope.download = (base64, name) => {
        download(base64, name);
      };

      $scope.clean = () => {
        $scope.message = "";
      };

      $scope.send = () => {
        var data = document.getElementById("message").value;

        var previousBlock = $scope.blockchain[$scope.blockchain.length - 1];

        var newBlock = {
          index: previousBlock.index + 1,
          name:  $scope.name,
          previousHash: previousBlock.hash,
          date: new Date(),
          data: $scope.crypto.encrypt($scope.message),
        };

        var hash = sha256.create().update(newBlock.index + newBlock.previousHash + newBlock.data).hex();
        newBlock.hash = hash;

        conn.invoke("Send", {
          newblock: newBlock,
          blockchain: $scope.blockchain
        });

        $scope.clean();
      };

      conn.start()
          .then(() => {
          console.log("Started");
      })
      .catch(err => {
          console.log("error")
      });
    };
  }]);
