# Cross-section

function curva_aprendizado()
    data = load('curva_aprendizado.csv');

    x = data(:, 1);
    z = data(:, 6);

    plot(x, z);
    title("Curva de aprendizado (Q-Learning)");
    xlabel("Número de episódios");
    ylabel("Ganho normalizado");
    pause();
endfunction