# Cross-section

function curva_aprendizado()
    data = load('coeff_var.csv');

    x = data(:, 1);
    z = data(:, 2);

    plot(x, z);
    title("Variabilidade da aptidão (50 amostras)");
    xlabel("Número de episódios (N)");
    ylabel("Coeficiente de variação (CV) %");
    pause();
endfunction