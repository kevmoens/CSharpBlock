var workspace;
export function initialize() {
    workspace = Blockly.inject('blocklyDiv', {
        media: 'media/',
        toolbox: document.getElementById('toolbox')
    });

    Blockly.Xml.domToWorkspace(document.getElementById('defaultProgram'), workspace);
}

export function evalProgram() {
    setOutput("");
    var xml = Blockly.Xml.workspaceToDom(workspace);
    var xmlText = new XMLSerializer().serializeToString(xml);
    send(xmlText, setOutput)
}

export function setOutput(value) {
    document.getElementById("output").innerText = value;
}

export function send(payload, cb) {
    DotNet.invokeMethodAsync('CSharpBlock', 'Evaluate', payload)
        .then(cb)
        .catch(err => console.error(err))
}
