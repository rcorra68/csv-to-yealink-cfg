const fs = require('fs');

module.exports.readVersion = function (contents) {
  const match = contents.match(/<Version>(.*)<\/Version>/);
  return match ? match[1] : '1.0.0';
};

module.exports.writeVersion = function (contents, version) {
  return contents.replace(
    /<Version>(.*)<\/Version>/,
    `<Version>${version}</Version>`
  );
};
